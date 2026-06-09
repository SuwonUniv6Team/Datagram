using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Datagram
{
    public partial class Form1 : Form
    {
        private List<FrameData> frames = new List<FrameData>();
        private List<FrameData> originalFrames = new List<FrameData>();  // 원본 데이터 보관용
        private string currentFolder = "";
        private Timer playbackTimer;
        private float playbackSpeed = 1.0f;
        private bool isUserInteracting = false;
        private bool isPlaybackActive = false;  // 재생 중 플래그
        private GraphWindow _graphWindow;
        private double _currentAngle = 0.0;
        private string _currentImageName = null;
        private Dictionary<string, double> _predictions = new Dictionary<string, double>();
        private string _loadedModelPath = null;
        private Process _trainProcess;
        private bool _isTraining = false;

        // 범위 선택 기능 관련 변수
        private int rangeStartIndex = -1;
        private bool isSelectingRangeStart = true;
        private readonly List<DeleteRange> deleteRanges = new List<DeleteRange>();

        public Form1()
        {
            InitializeComponent();
            KeyPreview = true;
            KeyDown += Form1_KeyDown;
            btnLoad.Click += BtnLoad_Click;
            lstFrames.SelectedIndexChanged += LstFrames_SelectedIndexChanged;
            lstFrames.MouseUp += LstFrames_MouseUp;  // 마우스 업 이벤트 추가 (다중선택용)
            trackFrame.Scroll += TrackFrame_Scroll;

            // 재생 제어 버튼 이벤트 등록
            btnplay.Click += BtnPlay_Click;
            btnpurse.Click += BtnPause_Click;
            btnnext.Click += BtnNext_Click;
            btntrace.Click += BtnTrace_Click;
            btnFilter.Click += BtnFilter_Click;
            btnDelete.Click += BtnDelete_Click;
            btnTrain.Click += BtnTrain_Click;
            picMain.Paint += PicMain_Paint;
            btnAIreview.Click += BtnAIReview_Click;
            btnAIreview.Text = "AI 검증";
            btnGraph.Click += BtnGraph_Click;

            // 다중 선택 모드 활성화 (Ctrl/Shift + 클릭으로 다중 선택 가능)
            lstFrames.SelectionMode = SelectionMode.MultiExtended;

            // Timer 초기화 (약 20 FPS 설정)
            playbackTimer = new Timer();
            playbackTimer.Interval = 50; // 50ms 마다 틱
            playbackTimer.Tick += PlaybackTimer_Tick;

            // 배속 콤보박스 초기화
            InitializeSpeedComboBox();

            // 필터 초기화 버튼 이벤트 등록
            btnreset.Click += BtnResetFilter_Click;

            // nud1 (급커브 수치) 초기화 - 소수점 0.1씩 증가/감소
            nud1.Increment = (decimal)0.1;
            nud1.DecimalPlaces = 1;

            // 범위 선택 버튼 이벤트 등록
            btnRangeSelect.Click += BtnRangeSelect_Click;
            lstDeleteRanges.SelectionMode = SelectionMode.MultiExtended;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsEditingInput()) return;

            if (e.KeyCode == Keys.Space)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                TogglePlayback();
            }
            else if (e.KeyCode == Keys.R)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                BtnRangeSelect_Click(null, EventArgs.Empty);
            }
        }

        private bool IsEditingInput()
        {
            Control control = ActiveControl;
            while (control is ContainerControl container && container.ActiveControl != null)
                control = container.ActiveControl;

            return control is TextBoxBase
                || control is NumericUpDown
                || control is ComboBox;
        }

        private void TogglePlayback()
        {
            if (isPlaybackActive || playbackTimer.Enabled)
                BtnPause_Click(this, EventArgs.Empty);
            else
                BtnPlay_Click(this, EventArgs.Empty);
        }

        private class DeleteRange
        {
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }

            public int Count => EndIndex - StartIndex + 1;

            public override string ToString()
            {
                return $"[{StartIndex:D6} ~ {EndIndex:D6}] {Count}개";
            }
        }

        private void InitializeSpeedComboBox()
        {
            cbboxspeed.Items.Clear();
            cbboxspeed.Items.Add("1x");
            cbboxspeed.Items.Add("2x");
            cbboxspeed.Items.Add("4x");
            cbboxspeed.Items.Add("8x");
            cbboxspeed.SelectedIndex = 0; // 기본값 1x
            cbboxspeed.SelectedIndexChanged += CbboxSpeed_SelectedIndexChanged;
        }

        private void CbboxSpeed_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbboxspeed.SelectedIndex)
            {
                case 0: // 1x
                    playbackSpeed = 1.0f;
                    break;
                case 1: // 2x
                    playbackSpeed = 2.0f;
                    break;
                case 2: // 4x
                    playbackSpeed = 4.0f;
                    break;
                case 3: // 8x
                    playbackSpeed = 8.0f;
                    break;
                default:
                    playbackSpeed = 1.0f;
                    break;
            }
            AddLog($"재생 속도 설정: {cbboxspeed.SelectedItem}");
        }

        private void BtnGraph_Click(object sender, EventArgs e)
        {
            if (_graphWindow == null || _graphWindow.IsDisposed)
            {
                _graphWindow = new GraphWindow();
                _graphWindow.Show(this);

                // 이미 폴더가 로드된 상태면 데이터 전달
                if (frames.Count > 0)
                    _graphWindow.LoadFromFolder(currentFolder, frames);
            }
            else
            {
                _graphWindow.Focus();
            }
        }

        private async void BtnTrain_Click(object sender, EventArgs e)
        {
            if (_isTraining)
            {
                try
                {
                    if (_trainProcess != null && !_trainProcess.HasExited)
                    {
                        _trainProcess.Kill();
                        AddLog("⛔ 학습이 중단되었습니다.");
                    }
                }
                catch { }
                _isTraining = false;
                btnTrain.Text = "▶ AI 학습 시작";
                btnTrain.BackColor = Color.FromArgb(0, 180, 100);
                btnTrain.Enabled = true;
                return;
            }

            string imageFolder = PrepareTrainingFolder();
            if (imageFolder == null) return;

            if (!Directory.Exists(imageFolder))
            {
                MessageBox.Show("폴더가 존재하지 않습니다.");
                return;
            }

            // 버튼 비활성화 (중복 클릭 방지)
            _isTraining = true;
            btnTrain.Text = "⛔ 학습 중지";
            btnTrain.BackColor = Color.FromArgb(200, 60, 60);
            btnTrain.Enabled = true;
            AddLog("환경 확인 중...");
            if (_graphWindow == null || _graphWindow.IsDisposed)
            {
                _graphWindow = new GraphWindow();
                _graphWindow.Show(this);
            }
            // 현재 로드된 데이터 전달
            if (frames.Count > 0)
                _graphWindow.LoadFromFolder(currentFolder, frames);
            _graphWindow.Focus();

            // 백그라운드에서 설치 확인 및 설치 진행
            bool ready = await Task.Run(() =>
            {
                var starter = new TrainStarter(txtLog);
                return starter.EnsureReady();
            });

            // 설치 확인 중에 중지 버튼 눌렀으면 여기서 중단
            if (!_isTraining)
            {
                AddLog("⛔ 학습이 취소되었습니다.");
                btnTrain.Text = "▶ AI 학습 시작";
                btnTrain.BackColor = Color.FromArgb(0, 180, 100);
                return;
            }

            if (!ready)
            {
                _isTraining = false;
                btnTrain.Text = "▶ AI 학습 시작";
                btnTrain.BackColor = Color.FromArgb(0, 180, 100);
                btnTrain.Enabled = true;
                AddLog("❌ 환경 준비 실패. 학습을 시작할 수 없습니다.");
                return;
            }

            // 설치 완료 후 학습 시작
            string scriptPath = GetScriptPath("train.py");
            if (scriptPath == null)
            {
                MessageBox.Show("train.py 파일을 찾을 수 없습니다.");
                btnTrain.Enabled = true;
                return;
            }

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{scriptPath}\" --image_folder \"{imageFolder}\" --epochs {(int)nudEpochs.Value}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(scriptPath)
            };

            Process process = new Process();
            process.StartInfo = psi;
            process.EnableRaisingEvents = true;

            process.OutputDataReceived += (s, args) =>
            {
                if (string.IsNullOrEmpty(args.Data)) return;
                string line = args.Data;

                Invoke(new Action(() =>
                {
                    bool appended = false;   // 메인 로그창에 실제로 출력했는지 (스크롤 흔들림 방지)

                    // [PROGRESS] 는 epoch(회차) 결과 → 한글로 변환 + 그래프 업데이트
                    if (line.StartsWith("[PROGRESS]"))
                    {
                        try
                        {
                            var parts = line.Substring("[PROGRESS]".Length).Trim().Split(' ');
                            int epoch = 0, totalEpochs = 30;
                            double loss = 0, valLoss = 0;
                            foreach (var part in parts)
                            {
                                var kv = part.Split('=');
                                if (kv.Length < 2) continue;
                                switch (kv[0])
                                {
                                    case "epoch":
                                        var ep = kv[1].Split('/');
                                        epoch = int.Parse(ep[0]);
                                        totalEpochs = int.Parse(ep[1]);
                                        break;
                                    case "loss":
                                        loss = double.Parse(kv[1], System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "val_loss":
                                        valLoss = double.Parse(kv[1], System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                }
                            }

                            // 한글로 출력
                            txtLog.AppendText($"[학습 {epoch}/{totalEpochs}회] 학습오차: {loss:F4}  검증오차: {valLoss:F4}{Environment.NewLine}");
                            appended = true;

                            // 그래프 창 업데이트
                            if (_graphWindow != null && !_graphWindow.IsDisposed)
                                _graphWindow.AddTrainEpoch(epoch, totalEpochs, loss, valLoss);
                        }
                        catch
                        {
                            txtLog.AppendText(line + Environment.NewLine);
                            appended = true;
                        }
                    }
                    else if (line.StartsWith("[DONE]"))
                    {
                        txtLog.AppendText($"✅ 학습 완료!{Environment.NewLine}");
                        appended = true;
                        if (_graphWindow != null && !_graphWindow.IsDisposed)
                        {
                            _graphWindow.TrainFinished();
                            _graphWindow.ShowAngleDistribution(frames);
                        }
                    }
                    // [LOG], [ERROR], [GRAPH], 태그 없는 줄 → 메인 로그창에는 표시 안 함
                    // (그래프 옆 원본 로그창에서 확인 가능)

                    // 실제로 출력했을 때만 스크롤 (흔들림 방지)
                    if (appended)
                    {
                        txtLog.SelectionStart = txtLog.Text.Length;
                        txtLog.ScrollToCaret();
                    }

                    // 그래프 창 로그 섹터에 원본 출력 전달
                    if (_graphWindow != null && !_graphWindow.IsDisposed)
                        _graphWindow.AppendRawLog(line);
                }));

            };

            process.ErrorDataReceived += (s, args) =>
            {
                if (string.IsNullOrEmpty(args.Data)) return;
                string err = args.Data.Trim();

                Invoke(new Action(() =>
                {
                    // 그래프 로그창에는 필터링 없이 원본 그대로 표시
                    if (_graphWindow != null && !_graphWindow.IsDisposed)
                        _graphWindow.AppendRawLog($"[STDERR] {err}");

                    // 메인 로그창에는 stderr 숨김 (그래프 옆 원본 로그창에서 확인 가능)
                }));
            };

            process.Exited += (s, args) =>
            {
                Invoke(new Action(() =>
                {
                    _isTraining = false;
                    btnTrain.Text = "▶ AI 학습 시작";
                    btnTrain.BackColor = Color.FromArgb(0, 180, 100);
                    btnTrain.Enabled = true;
                }));
            };

            // 그래프 창 로그 섹터 초기화
            if (_graphWindow != null && !_graphWindow.IsDisposed)
                _graphWindow.StartLogSession();

            process.Start();
            _trainProcess = process;
            AddLog($"AI 학습 시작: {scriptPath}");
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        private void AddLog(string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            txtLog.AppendText($"[{time}] {message}\r\n");
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }

        // Keras verbose=1 진행바 줄인지 판별 (예: " 97/97 [====] - 12s 98ms/step - loss: ...")
        private bool IsKerasProgressLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return false;
            string s = line.Trim();
            // 진행바 본체 / ETA / step 속도 표기 / epoch 헤더
            if (s.Contains("[=") || s.Contains("[.") || s.Contains("ETA:") ||
                s.Contains("step") || s.Contains("us/step") || s.Contains("ms/step"))
                return true;
            // "Epoch 1/30" 영어 헤더 (한글 "[학습 x/30회]"와 중복)
            if (s.StartsWith("Epoch ")) return true;
            // " 12/97" 처럼 '숫자/숫자'로 시작하는 진행 표기
            return System.Text.RegularExpressions.Regex.IsMatch(s, @"^\d+/\d+");
        }

        private void BtnPlay_Click(object sender, EventArgs e)
        {
            if (frames.Count == 0) return;

            // 이미 마지막 프레임인 경우 처음부터 다시 재생
            if (GetCurrentFrameIndex() >= frames.Count - 1)
            {
                lstFrames.SelectedIndexChanged -= LstFrames_SelectedIndexChanged;
                lstFrames.ClearSelected();
                if (lstFrames.Items.Count > 0)
                    lstFrames.SelectedIndex = 0;
                lstFrames.SelectedIndexChanged += LstFrames_SelectedIndexChanged;
            }

            isPlaybackActive = true;
            AddLog("▶ 자동재생 시작");
            playbackTimer.Start();
        }

        private void BtnPause_Click(object sender, EventArgs e)
        {
            isPlaybackActive = false;
            AddLog("⏸ 자동재생 정지");
            playbackTimer.Stop();
        }

        private int GetCurrentFrameIndex()
        {
            if (frames.Count == 0) return 0;

            if (trackFrame.Value >= 0 && trackFrame.Value < frames.Count)
                return trackFrame.Value;

            int selectedIndex = lstFrames.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < frames.Count)
                return selectedIndex;

            return 0;
        }

        private void ScrollFrameListTo(int index)
        {
            if (lstFrames.Items.Count == 0) return;

            int safeIndex = Math.Max(0, Math.Min(index, lstFrames.Items.Count - 1));
            try
            {
                lstFrames.TopIndex = safeIndex;
            }
            catch
            {
                // TopIndex can throw while the list is being rebuilt.
            }
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            playbackTimer.Stop();
            isPlaybackActive = false;

            if (frames.Count == 0) return;

            int currentIdx = GetCurrentFrameIndex();

            // 현재 프레임이 마지막이 아니면 다음으로 이동
            if (currentIdx < frames.Count - 1)
            {
                int nextIdx = currentIdx + 1;

                // 이벤트 임시 제거
                lstFrames.SelectedIndexChanged -= LstFrames_SelectedIndexChanged;
                lstFrames.ClearSelected();
                lstFrames.SelectedIndex = nextIdx;
                lstFrames.SelectedIndexChanged += LstFrames_SelectedIndexChanged;

                // 트랙바 동기화
                trackFrame.ValueChanged -= TrackFrame_Scroll;
                trackFrame.Value = nextIdx;
                trackFrame.ValueChanged += TrackFrame_Scroll;

                // 이미지 표시
                ShowFrame(frames[nextIdx]);
                AddLog($"▶ 다음 프레임: {nextIdx}번 이동");
            }
            else
            {
                AddLog($"ⓘ 마지막 프레임입니다");
            }
        }

        private void BtnTrace_Click(object sender, EventArgs e)
        {
            playbackTimer.Stop();
            isPlaybackActive = false;

            if (frames.Count == 0) return;

            int currentIdx = GetCurrentFrameIndex();

            // 현재 프레임이 첫 번째가 아니면 이전으로 이동
            if (currentIdx > 0)
            {
                int prevIdx = currentIdx - 1;

                // 이벤트 임시 제거
                lstFrames.SelectedIndexChanged -= LstFrames_SelectedIndexChanged;
                lstFrames.ClearSelected();
                lstFrames.SelectedIndex = prevIdx;
                lstFrames.SelectedIndexChanged += LstFrames_SelectedIndexChanged;

                // 트랙바 동기화
                trackFrame.ValueChanged -= TrackFrame_Scroll;
                trackFrame.Value = prevIdx;
                trackFrame.ValueChanged += TrackFrame_Scroll;

                // 이미지 표시
                ShowFrame(frames[prevIdx]);
                AddLog($"◀ 이전 프레임: {prevIdx}번 이동");
            }
            else
            {
                AddLog($"ⓘ 첫 번째 프레임입니다");
            }
        }

        private void PlaybackTimer_Tick(object sender, EventArgs e)
        {
            if (frames.Count == 0)
            {
                playbackTimer.Stop();
                isPlaybackActive = false;
                return;
            }

            // 사용자가 상호작용 중이면 다음 틱 때까지 대기
            if (isUserInteracting)
            {
                isUserInteracting = false;
                return;
            }

            try
            {
                int currentIdx = GetCurrentFrameIndex();

                int nextIdx = currentIdx + (int)playbackSpeed;

                if (nextIdx < frames.Count)
                {
                    if (isSelectingRangeStart)
                    {
                        lstFrames.SelectedIndexChanged -= LstFrames_SelectedIndexChanged;
                        lstFrames.ClearSelected();
                        lstFrames.SelectedIndex = nextIdx;
                        lstFrames.SelectedIndexChanged += LstFrames_SelectedIndexChanged;
                    }

                    trackFrame.ValueChanged -= TrackFrame_Scroll;
                    trackFrame.Value = nextIdx;
                    trackFrame.ValueChanged += TrackFrame_Scroll;

                    ScrollFrameListTo(nextIdx);
                    ShowFrame(frames[nextIdx]);
                }
                else
                {
                    playbackTimer.Stop(); // 마지막 프레임에 도달하면 정지
                    isPlaybackActive = false;
                    AddLog("⏹ 재생 종료");
                }
            }
            catch (Exception ex)
            {
                AddLog($"⚠ 재생 중 오류: {ex.Message}");
                playbackTimer.Stop();
                isPlaybackActive = false;
            }
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            try
            {
                if (frames == null || frames.Count == 0) return;
                ResetRangeSelectionState(true);

                // 전체 데이터 리스트 (// 변수명 입력 필요)
                // IEnumerable<DonkeyFrame> query = 전체데이터리스트.AsEnumerable();
                var query = frames.AsEnumerable();

                if (cbox1.Checked)
                {
                    query = query.Where(f => Math.Abs(f.Angle) > 0.05);
                }

                if (cbox4.Checked)
                {
                    query = query.Where(f => f.Throttle > 0);
                }

                if (cbox2.Checked)
                {
                    double curveValue = (double)nud1.Value;
                    query = query.Where(f => Math.Abs(f.Angle) >= curveValue);
                }

                if (cbox3.Checked && cbboxtub.SelectedItem != null)
                {
                    string selectedCatalog = cbboxtub.SelectedItem.ToString();
                    // "[ 전체 주행 데이터 보기 ]"가 아닌 경우만 필터링 적용
                    if (!selectedCatalog.Contains("전체"))
                    {
                        query = query.Where(f => f.CatalogName == selectedCatalog);
                    }
                }

                // 필터링된 내용을 리스트로 변환 (// 변수명 입력 필요)
                // List<DonkeyFrame> filteredList = query.ToList();
                var filteredList = query.ToList();

                // UI 갱신 메서드 (// 변수명 입력 필요)
                // BindDataToUI(filteredList);

                // --- 아래는 현재 코드베이스와 호환을 위한 기존 UI 갱신 인라인 로직 ---
                frames = filteredList;
                lstFrames.Items.Clear();
                for (int i = 0; i < frames.Count; i++)
                {
                    frames[i].FrameIndex = i;
                    lstFrames.Items.Add(frames[i]);
                }

                if (frames.Count > 0)
                {
                    trackFrame.Maximum = frames.Count - 1;
                    if (trackFrame.Maximum >= 0 && frames.Count > 0)
                        trackFrame.Value = 0;
                    if (lstFrames.Items.Count > 0)
                        lstFrames.SelectedIndex = 0;
                }
                else
                {
                    trackFrame.Maximum = 0;
                    if (trackFrame.Maximum >= 0 && frames.Count > 0)
                        trackFrame.Value = 0;
                    if (picMain.Image != null)
                    {
                        picMain.Image.Dispose();
                        picMain.Image = null;
                    }
                    lblAngleName.Text = "Angle: ";
                    lblThrottleName.Text = "Throttle: ";
                    prgAngle.Value = 0;
                    prgThrottle.Value = 0;
                }

                // 최하단 로그창 텍스트박스(txtLog)에 실시간 처리 결과 반영
                string time = DateTime.Now.ToString("HH:mm:ss");
                txtLog.AppendText($"[{time}] 필터 적용 완료: {filteredList.Count}개의 프레임이 선택되었습니다." + Environment.NewLine);
                txtLog.SelectionStart = txtLog.Text.Length;
                txtLog.ScrollToCaret();
            }
            catch (Exception ex)
            {
                // 데이터 처리 중 에러 발생 시 로그 반영
                string errorTime = DateTime.Now.ToString("HH:mm:ss");
                txtLog.AppendText($"[{errorTime}] ❌ 필터 적용 에러 발생: {ex.Message}" + Environment.NewLine);
                txtLog.SelectionStart = txtLog.Text.Length;
                txtLog.ScrollToCaret();
            }
        }

        private void BtnResetFilter_Click(object sender, EventArgs e)
        {
            try
            {
                ResetRangeSelectionState(true);

                // 1. 모든 체크박스를 Checked = false로 변경
                cbox1.Checked = false;
                cbox2.Checked = false;
                cbox3.Checked = false;
                cbox4.Checked = false;

                // 2. nud1의 Value를 기본값 0.6으로 변경
                nud1.Value = (decimal)0.6;

                // 3. cbboxtub의 SelectedIndex를 0으로 설정 (첫 번째 항목 선택)
                if (cbboxtub.Items.Count > 0)
                {
                    cbboxtub.SelectedIndex = 0;
                }

                // 4. 원본 데이터로 UI 갱신
                frames = new List<FrameData>(originalFrames);
                lstFrames.Items.Clear();
                for (int i = 0; i < frames.Count; i++)
                {
                    frames[i].FrameIndex = i;
                    lstFrames.Items.Add(frames[i]);
                }

                if (frames.Count > 0)
                {
                    trackFrame.Maximum = frames.Count - 1;
                    if (trackFrame.Maximum >= 0 && frames.Count > 0)
                        trackFrame.Value = 0;
                    if (lstFrames.Items.Count > 0)
                        lstFrames.SelectedIndex = 0;
                }
                else
                {
                    trackFrame.Maximum = 0;
                    if (trackFrame.Maximum >= 0 && frames.Count > 0)
                        trackFrame.Value = 0;
                    if (picMain.Image != null)
                    {
                        picMain.Image.Dispose();
                        picMain.Image = null;
                    }
                    lblAngleName.Text = "Angle: ";
                    lblThrottleName.Text = "Throttle: ";
                    prgAngle.Value = 0;
                    prgThrottle.Value = 0;
                }

                // 5. 로그 메시지 추가
                string time = DateTime.Now.ToString("HH:mm:ss");
                txtLog.AppendText($"[{time}] 필터가 초기화되었습니다. 원본 데이터를 표시합니다." + Environment.NewLine);
                txtLog.SelectionStart = txtLog.Text.Length;
                txtLog.ScrollToCaret();
            }
            catch (Exception ex)
            {
                // 에러 발생 시 로그 반영
                string errorTime = DateTime.Now.ToString("HH:mm:ss");
                txtLog.AppendText($"[{errorTime}] ❌ 필터 초기화 에러 발생: {ex.Message}" + Environment.NewLine);
                txtLog.SelectionStart = txtLog.Text.Length;
                txtLog.ScrollToCaret();
            }
        }

        /// <summary>
        /// 마우스 업 이벤트 (다중선택 완료 후)
        /// 재생 중이 아닐 때만 마지막 선택된 프레임을 표시
        /// </summary>
        private void LstFrames_MouseUp(object sender, MouseEventArgs e)
        {
            // 재생 중이면 아무것도 하지 않음
            if (isPlaybackActive)
            {
                AddLog("ⓘ 재생 중에는 다중선택이 비활성화됩니다.");
                return;
            }

            // 다중 선택된 항목이 있으면 마지막 선택 항목을 표시
            if (lstFrames.SelectedIndices.Count > 0)
            {
                int lastSelected = lstFrames.SelectedIndices[lstFrames.SelectedIndices.Count - 1];

                // 트랙바 업데이트
                trackFrame.ValueChanged -= TrackFrame_Scroll;
                trackFrame.Value = lastSelected;
                trackFrame.ValueChanged += TrackFrame_Scroll;

                // 이미지 표시
                if (lastSelected >= 0 && lastSelected < frames.Count)
                {
                    ShowFrame(frames[lastSelected]);
                    AddLog($"✓ 다중선택: {lstFrames.SelectedIndices.Count}개 프레임 선택됨");
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (frames.Count == 0) return;

            var selectedIndices = GetIndicesSelectedForDeletion()
                .Where(i => i >= 0 && i < frames.Count)
                .Distinct()
                .OrderByDescending(i => i)
                .ToList();

            if (selectedIndices.Count == 0) return;

            var selectedRangeIndices = lstDeleteRanges.SelectedIndices
                .Cast<int>()
                .Where(i => i >= 0 && i < deleteRanges.Count)
                .ToList();

            var result = MessageBox.Show($"선택된 {selectedIndices.Count}개 프레임을 삭제하시겠습니까?\n(이미지와 데이터가 완전히 삭제됩니다)", 
                "프레임 삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // 0. 한 번에 이미지 메모리 해제
                AddLog("🔓 메모리에서 이미지 해제 중...");
                try
                {
                    if (picMain.Image != null)
                    {
                        picMain.Image.Dispose();
                        picMain.Image = null;
                    }
                    AddLog("✓ 메모리 정리 완료");
                }
                catch (Exception ex)
                {
                    AddLog($"⚠ 메모리 해제 중 오류: {ex.Message}");
                }

                // 인덱스가 꼬이지 않도록 내림차순 정렬 후 삭제
                int nextIndex = selectedIndices.Min();

                int imageDeletedCount = 0;
                int imageDeletionFailedCount = 0;

                // 1. 이미지 파일 삭제
                foreach (int idx in selectedIndices)
                {
                    try
                    {
                        if (DeleteImageFile(frames[idx]))
                        {
                            imageDeletedCount++;
                        }
                        else
                        {
                            imageDeletionFailedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        AddLog($"이미지 삭제 실패 {idx}: {ex.Message}");
                        imageDeletionFailedCount++;
                    }
                }

                // 2. 카탈로그/레코드 파일 배치 처리 (한 번의 파일 I/O로 모든 프레임 처리)
                int recordDeletedCount = 0;
                try
                {
                    var framesToDelete = selectedIndices.Select(idx => frames[idx]).ToList();
                    recordDeletedCount = DeleteRecordDataBatch(framesToDelete);
                }
                catch (Exception ex)
                {
                    AddLog($"⚠ 레코드 파일 처리 오류: {ex.Message}");
                }

                // 3. frames 리스트에서 삭제
                foreach (int idx in selectedIndices)
                {
                    frames.RemoveAt(idx);
                }

                // 삭제 결과 메시지
                AddLog($"━━━ 프레임 삭제 결과 ━━━");
                AddLog($"  ✓ 이미지 삭제: {imageDeletedCount}개");
                if (imageDeletionFailedCount > 0)
                {
                    AddLog($"  ✗ 이미지 삭제 실패: {imageDeletionFailedCount}개");
                    AddLog($"  💡 팁: OneDrive 또는 클라우드 동기화를 사용 중이면 대기하세요.");
                }
                AddLog($"  ✓ 레코드 데이터: {recordDeletedCount}개 삭제");
                AddLog($"━━━━━━━━━━━━━━━━━━");

                // 리스트 및 프레임 번호 갱신
                lstFrames.Items.Clear();
                for (int i = 0; i < frames.Count; i++)
                {
                    frames[i].FrameIndex = i;
                    lstFrames.Items.Add(frames[i]);
                }

                if (frames.Count > 0)
                {
                    // 트랙바 범위 설정
                    trackFrame.Minimum = 0;
                    trackFrame.Maximum = frames.Count - 1;

                    // 다음 인덱스 범위 보정
                    if (nextIndex >= frames.Count)
                        nextIndex = frames.Count - 1;
                    if (nextIndex < 0)
                        nextIndex = 0;

                    lstFrames.SelectedIndexChanged -= LstFrames_SelectedIndexChanged;
                    lstFrames.SelectedIndex = nextIndex;
                    trackFrame.Value = nextIndex;
                    lstFrames.SelectedIndexChanged += LstFrames_SelectedIndexChanged;
                    ShowFrame(frames[nextIndex]);
                }
                else
                {
                    trackFrame.Maximum = 0;
                    if (trackFrame.Maximum >= 0 && frames.Count > 0)
                        trackFrame.Value = 0;
                    if (picMain.Image != null)
                    {
                        picMain.Image.Dispose();
                        picMain.Image = null;
                    }
                    lblAngleName.Text = "Angle: ";
                    lblThrottleName.Text = "Throttle: ";
                    prgAngle.Value = 0;
                    prgThrottle.Value = 0;
                }

                // 원본 데이터도 동기화
                originalFrames = new List<FrameData>(frames);
                UpdateDeleteRangesAfterDeletion(selectedIndices, selectedRangeIndices);
                ResetRangeSelectionState();
            }
        }

        /// <summary>
        /// 이미지 파일 삭제 (최적화 버전)
        /// </summary>
        private bool DeleteImageFile(FrameData frame)
        {
            string imgPath = null;
            try
            {
                imgPath = Path.Combine(currentFolder, "images", frame.ImagePath);

                if (!File.Exists(imgPath))
                {
                    AddLog($"⚠ 이미지 파일을 찾을 수 없음: {frame.ImagePath}");
                    return false;
                }

                // 파일 삭제 시도 (최대 3회, 짧은 대기)
                int maxRetries = 3;
                int retryCount = 0;
                bool deleted = false;

                while (!deleted && retryCount < maxRetries)
                {
                    try
                    {
                                    // _deleted 폴더로 이동
                        string deletedFolder = Path.Combine(currentFolder, "_deleted", "images");
                        Directory.CreateDirectory(deletedFolder);
                        string destPath = Path.Combine(deletedFolder, Path.GetFileName(imgPath));
                                    // 같은 이름 있으면 덮어쓰기
                        if (File.Exists(destPath)) File.Delete(destPath);
                        File.Move(imgPath, destPath);
                        deleted = true;
                        AddLog($"✓ 이미지 이동: {frame.ImagePath} → _deleted/images/");
                        return true;
                    }
                    catch (IOException ioEx)
                    {
                        retryCount++;
                        if (retryCount < maxRetries)
                        {
                            // 최소 대기만 (50ms * retryCount)
                            System.Threading.Thread.Sleep(50 * retryCount);
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                return false;
            }
            catch (UnauthorizedAccessException)
            {
                AddLog($"✗ 파일 접근 권한 없음: {frame.ImagePath}");
                return false;
            }
            catch (IOException ioEx)
            {
                AddLog($"✗ 파일 잠금: {Path.GetFileName(frame.ImagePath)}");
                return false;
            }
            catch (Exception ex)
            {
                AddLog($"✗ 이미지 삭제 실패: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 여러 프레임의 레코드 데이터를 배치 처리로 제거 (최적화: 파일 I/O 횟수 최소화)
        /// </summary>
        private int DeleteRecordDataBatch(List<FrameData> framesToDelete)
        {
            if (framesToDelete == null || framesToDelete.Count == 0)
                return 0;

            try
            {
                // 삭제할 이미지 파일명 집합 (빠른 검색)
                var imageFileNamesToDelete = new HashSet<string>(
                    framesToDelete.Select(f => Path.GetFileName(f.ImagePath)),
                    StringComparer.OrdinalIgnoreCase
                );

                var imagePathsToDelete = new HashSet<string>(
                    framesToDelete.Select(f => f.ImagePath),
                    StringComparer.OrdinalIgnoreCase
                );

                // Catalog 파일들 한 번에 찾기
                string[] catalogFiles = Directory.GetFiles(currentFolder, "*catalog*", SearchOption.TopDirectoryOnly);
                string[] recordFiles = Directory.GetFiles(currentFolder, "record_*.json", SearchOption.TopDirectoryOnly);
                string[] jsonFiles = Directory.GetFiles(currentFolder, "*.json", SearchOption.TopDirectoryOnly);

                var allFiles = catalogFiles.Concat(recordFiles).Concat(jsonFiles).Distinct().ToArray();

                if (allFiles.Length == 0)
                    return 0;

                int deletedCount = 0;

                // 각 파일을 한 번만 읽고 쓰기
                foreach (string filePath in allFiles)
                {
                    try
                    {
                        if (Path.GetExtension(filePath).ToLower() == ".json")
                        {
                            if (DeleteFromJsonFileBatch(filePath, imageFileNamesToDelete, imagePathsToDelete))
                                deletedCount += framesToDelete.Count;
                        }
                        else
                        {
                            if (DeleteFromTextFileBatch(filePath, imageFileNamesToDelete, imagePathsToDelete))
                                deletedCount += framesToDelete.Count;
                        }
                    }
                    catch (Exception ex)
                    {
                        AddLog($"⚠ 파일 처리 오류 ({Path.GetFileName(filePath)}): {ex.Message}");
                        continue;
                    }
                }

                return deletedCount;
            }
            catch (Exception ex)
            {
                AddLog($"✗ 배치 처리 오류: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// JSON 파일에서 여러 프레임 배치 삭제 (정확한 경로 매칭)
        /// </summary>
        private bool DeleteFromJsonFileBatch(string jsonPath, HashSet<string> imageFileNames, HashSet<string> imagePaths)
        {
            try
            {
                var lines = File.ReadAllLines(jsonPath);
                var originalLineCount = lines.Length;

                // 모든 삭제 대상 이미지를 포함한 라인 제거 (정확한 경로 기반)
                var filteredLines = lines.Where(line =>
                {
                    // imagePaths 기준으로 먼저 확인 (더 정확함)
                    foreach (var imagePath in imagePaths)
                    {
                        // JSON 파일에서는 경로가 따옴표로 감싸져 있을 수 있음
                        // "test/image_001.jpg" 형태로 매칭
                        if (line.Contains($"\"{imagePath}\"") || 
                            line.Contains($"'{imagePath}'") ||
                            (line.Contains(imagePath) && IsValidPathMatch(line, imagePath)))
                            return false;
                    }

                    // imageFileNames로 2차 확인 (파일명만)
                    foreach (var fileName in imageFileNames)
                    {
                        // 파일명 경계를 확인하여 부분 매칭 방지
                        // "test_001.jpg" 검색 시 test_0010.jpg와 구분
                        if (IsFileNameInLine(line, fileName))
                            return false;
                    }

                    return true;
                }).ToArray();

                if (originalLineCount != filteredLines.Length)
                {
                    filteredLines = FixJsonStructure(filteredLines);

                    using (var writer = new StreamWriter(jsonPath, false, Encoding.UTF8, 4096))
                    {
                        foreach (var line in filteredLines)
                        {
                            writer.WriteLine(line);
                        }
                    }
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                AddLog($"✗ JSON 배치 삭제 오류: {Path.GetFileName(jsonPath)}");
                return false;
            }
        }

        /// <summary>
        /// 텍스트 파일에서 여러 프레임 배치 삭제 (정확한 경로 매칭)
        /// </summary>
        private bool DeleteFromTextFileBatch(string catalogPath, HashSet<string> imageFileNames, HashSet<string> imagePaths)
        {
            try
            {
                var lines = File.ReadAllLines(catalogPath);
                var originalLineCount = lines.Length;

                // 모든 삭제 대상 이미지를 포함한 라인 제거 (정확한 경로 기반)
                var filteredLines = lines.Where(line =>
                {
                    // imagePaths 기준으로 먼저 확인 (더 정확함)
                    foreach (var imagePath in imagePaths)
                    {
                        if (line.Contains(imagePath) && IsValidPathMatch(line, imagePath))
                            return false;
                    }

                    // imageFileNames로 2차 확인 (파일명만)
                    foreach (var fileName in imageFileNames)
                    {
                        // 파일명 경계를 확인하여 부분 매칭 방지
                        if (IsFileNameInLine(line, fileName))
                            return false;
                    }

                    return true;
                }).ToArray();

                if (originalLineCount != filteredLines.Length)
                {
                               // 삭제된 라인을 _deleted 폴더에 백업
                    string deletedFolder = Path.Combine(currentFolder, "_deleted");
                    Directory.CreateDirectory(deletedFolder);
                    string backupCatalog = Path.Combine(deletedFolder, Path.GetFileName(catalogPath));
                    var deletedLines = lines.Except(filteredLines).ToArray();
                    File.AppendAllLines(backupCatalog, deletedLines, Encoding.UTF8);

                              // 원본 catalog 업데이트
                    using (var writer = new StreamWriter(catalogPath, false, Encoding.UTF8, 4096))
                    {
                        foreach (var line in filteredLines)
                            writer.WriteLine(line);
                    }
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                AddLog($"✗ 텍스트 배치 삭제 오류: {Path.GetFileName(catalogPath)}");
                return false;
            }
        }

        /// <summary>
        /// 경로가 라인에 올바르게 포함되었는지 확인 (경계 검사)
        /// 예: "test_001.jpg"는 "test_0010.jpg"와 구분
        /// </summary>
        private bool IsValidPathMatch(string line, string imagePath)
        {
            // 경로의 시작과 끝에서 경계 문자 확인
            // JSON, 공백, 콤마, 따옴표 등으로 구분되어야 함
            int index = line.IndexOf(imagePath, StringComparison.OrdinalIgnoreCase);
            if (index < 0) return false;

            // 시작 경계 확인
            if (index > 0)
            {
                char prevChar = line[index - 1];
                // 파일명이 경계 문자로 시작되지 않으면 부분 매칭
                if (char.IsLetterOrDigit(prevChar) && prevChar != '\\' && prevChar != '/')
                    return false;
            }

            // 끝 경계 확인
            int endIndex = index + imagePath.Length;
            if (endIndex < line.Length)
            {
                char nextChar = line[endIndex];
                // 파일명이 경계 문자로 끝나지 않으면 부분 매칭
                if (char.IsLetterOrDigit(nextChar) && nextChar != '\\' && nextChar != '/')
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 파일명이 라인에 올바르게 포함되었는지 확인 (파일명 경계 검사)
        /// </summary>
        private bool IsFileNameInLine(string line, string fileName)
        {
            int index = line.IndexOf(fileName, StringComparison.OrdinalIgnoreCase);
            if (index < 0) return false;

            // 파일명 앞에 있는 문자 확인 (경계)
            if (index > 0)
            {
                char prevChar = line[index - 1];
                // 파일명 확장자 부분에서 겹칠 수 있으므로 엄격하게 검사
                if (char.IsLetterOrDigit(prevChar) || prevChar == '_' || prevChar == '-' || prevChar == '.')
                    return false;
            }

            // 파일명 뒤에 있는 문자 확인 (경계)
            int endIndex = index + fileName.Length;
            if (endIndex < line.Length)
            {
                char nextChar = line[endIndex];
                if (char.IsLetterOrDigit(nextChar) || nextChar == '_' || nextChar == '-' || nextChar == '.')
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Catalog/Record 파일에서 프레임 데이터 제거 (단일 파일용 - 호환성 유지)
        /// </summary>
        private bool DeleteRecordData(FrameData frame)
        {
            try
            {
                bool deleted = false;

                // Catalog 파일들 찾기
                string[] catalogFiles = Directory.GetFiles(currentFolder, "*catalog*", SearchOption.TopDirectoryOnly);
                string[] recordFiles = Directory.GetFiles(currentFolder, "record_*.json", SearchOption.TopDirectoryOnly);
                string[] jsonFiles = Directory.GetFiles(currentFolder, "*.json", SearchOption.TopDirectoryOnly);

                var allFiles = catalogFiles.Concat(recordFiles).Concat(jsonFiles).Distinct().ToArray();

                AddLog($"📁 찾은 파일: {allFiles.Length}개");
                foreach (var file in allFiles)
                {
                    AddLog($"   - {Path.GetFileName(file)}");
                }

                if (allFiles.Length == 0)
                {
                    AddLog($"⚠ Catalog/Record 파일을 찾을 수 없음");
                    return false;
                }

                foreach (string filePath in allFiles)
                {
                    try
                    {
                        if (Path.GetExtension(filePath).ToLower() == ".json")
                        {
                            // JSON 형식 파일 처리
                            if (DeleteFromJsonFile(filePath, frame))
                            {
                                deleted = true;
                            }
                        }
                        else
                        {
                            // 텍스트 형식 파일 처리
                            if (DeleteFromTextFile(filePath, frame))
                            {
                                deleted = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AddLog($"⚠ {Path.GetFileName(filePath)} 처리 중 오류: {ex.Message}");
                        continue;
                    }
                }

                return deleted;
            }
            catch (Exception ex)
            {
                AddLog($"✗ 레코드 파일 처리 실패: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// JSON 파일에서 프레임 데이터 제거 (최적화: 한 번의 파일 읽기)
        /// </summary>
        private bool DeleteFromJsonFile(string jsonPath, FrameData frame)
        {
            try
            {
                string imageFileName = Path.GetFileName(frame.ImagePath);

                // 라인 단위로 한 번에 읽고 필터링
                var lines = File.ReadAllLines(jsonPath);
                var originalLineCount = lines.Length;

                // 이미지 파일명을 포함한 라인 제거
                var filteredLines = lines.Where(line => !line.Contains(imageFileName)).ToArray();

                if (originalLineCount != filteredLines.Length)
                {
                    // JSON 구조 보정 (쉼표 처리)
                    filteredLines = FixJsonStructure(filteredLines);

                    // 스트리밍 방식으로 파일 쓰기 (메모리 효율화)
                    using (var writer = new StreamWriter(jsonPath, false, Encoding.UTF8, 4096))
                    {
                        foreach (var line in filteredLines)
                        {
                            writer.WriteLine(line);
                        }
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                AddLog($"✗ JSON 파일 처리 오류: {Path.GetFileName(jsonPath)}");
                return false;
            }
        }

        /// <summary>
        /// JSON 구조 보정 (쉼표 처리)
        /// </summary>
        private string[] FixJsonStructure(string[] lines)
        {
            if (lines.Length == 0) return lines;

            var fixedLines = new List<string>();
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                // 빈 라인 스킵
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // 마지막 줄이거나 다음 줄이 쉼표로 시작하지 않으면 쉼표 제거
                if (line.TrimEnd().EndsWith(","))
                {
                    if (i == lines.Length - 1 || (i + 1 < lines.Length && !lines[i + 1].TrimStart().StartsWith("}")))
                    {
                        line = line.TrimEnd().TrimEnd(',');
                    }
                }

                fixedLines.Add(line);
            }

            return fixedLines.ToArray();
        }

        /// <summary>
        /// 텍스트 형식 Catalog 파일에서 프레임 데이터 제거 (최적화)
        /// </summary>
        private bool DeleteFromTextFile(string catalogPath, FrameData frame)
        {
            try
            {
                string imageFileName = Path.GetFileName(frame.ImagePath);

                string[] lines = File.ReadAllLines(catalogPath);
                var originalLineCount = lines.Length;

                // 이미지 파일명을 포함한 라인 제거
                string[] filteredLines = lines.Where(line => 
                    !line.Contains(imageFileName) && 
                    !line.Contains(frame.ImagePath)
                ).ToArray();

                if (originalLineCount != filteredLines.Length)
                {
                    // 스트리밍 방식으로 파일 쓰기
                    using (var writer = new StreamWriter(catalogPath, false, Encoding.UTF8, 4096))
                    {
                        foreach (var line in filteredLines)
                        {
                            writer.WriteLine(line);
                        }
                    }
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                AddLog($"✗ 텍스트 파일 처리 오류: {Path.GetFileName(catalogPath)}");
                return false;
            }
        }


        private void BtnLoad_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    currentFolder = fbd.SelectedPath;
                    txtPath.Text = currentFolder;
                    LoadCatalog(currentFolder);
                    if (_graphWindow != null && !_graphWindow.IsDisposed)
                        _graphWindow.LoadFromFolder(currentFolder, frames);
                }
            }
        }

        private void LoadCatalog(string folderPath)
        {
            frames.Clear();
            lstFrames.Items.Clear();
            ResetRangeSelectionState(true);
            // 트랙바 먼저 초기화 (SelectedIndex 오류 방지)
            trackFrame.Minimum = 0;
            trackFrame.Maximum = 0;
            trackFrame.Value   = 0;
            AddLog("━━━ 카탈로그 로드 시작 ━━━");

            // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
            // [주행 회차 콤보박스 동적 매핑]
            // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
            try
            {
                // 1. 기존 콤보박스 항목 모두 제거 (중복 방지)
                cbboxtub.Items.Clear();

                // 2. 기본값 추가: "[ 전체 주행 데이터 보기 ]"
                cbboxtub.Items.Add("[ 전체 주행 데이터 보기 ]");

                // 3. .catalog 파일 탐색 및 콤보박스 채우기
                // 선택된 데이터 폴더 경로 지정 및 .catalog 파일 탐색 필요
                string[] catalogFiles = Directory.GetFiles(folderPath, "*.catalog");

                // 4. 파일명(확장자 제외)만 추출해서 콤보박스에 추가
                foreach (string catalogFile in catalogFiles)
                {
                    // 파일명만 추출 (예: "C:\path\catalog_0.catalog" → "catalog_0.catalog")
                    string fileName = Path.GetFileName(catalogFile);

                    // 확장자 제거 (예: "catalog_0.catalog" → "catalog_0")
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

                    // 콤보박스에 추가
                    cbboxtub.Items.Add(fileNameWithoutExtension);
                }

                // 5. 기본값 선택 (0번 인덱스 = "[ 전체 주행 데이터 보기 ]")
                cbboxtub.SelectedIndex = 0;

                // 로그 출력
                AddLog($"✓ 주행 회차 목록 로드: {catalogFiles.Length}개의 데이터 파일 발견");
                if (catalogFiles.Length > 0)
                {
                    foreach (string file in catalogFiles)
                    {
                        AddLog($"   ├─ {Path.GetFileNameWithoutExtension(file)}");
                    }
                }
            }
            catch (Exception ex)
            {
                AddLog($"⚠ 주행 회차 콤보박스 로드 중 오류: {ex.Message}");
                // 에러 발생 시에도 기본값만 유지
                cbboxtub.Items.Clear();
                cbboxtub.Items.Add("[ 전체 주행 데이터 보기 ]");
                cbboxtub.SelectedIndex = 0;
            }
            // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

            // catalog 파일 또는 json 파일 찾기
            var files = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(f => 
                {
                    string name = Path.GetFileName(f).ToLower();
                    return name.Contains("catalog") || name.StartsWith("record_") || name.EndsWith(".json");
                }).ToArray();

            if (files.Length == 0)
            {
                MessageBox.Show("선택한 폴더에서 catalog 파일을 찾을 수 없습니다.");
                AddLog("✗ Catalog 파일 없음");
                return;
            }

            AddLog($"📁 찾은 파일: {files.Length}개");
            foreach (var file in files)
            {
                AddLog($"   - {Path.GetFileName(file)}");
            }

            int totalLinesProcessed = 0;
            int framesLoaded = 0;

            foreach (var file in files)
            {
                try
                {
                    string[] lines = File.ReadAllLines(file);
                    AddLog($"\n📄 {Path.GetFileName(file)}: {lines.Length}개 라인");

                    // 현재 파일의 카탈로그 이름 추출 (확장자 제거)
                    string currentCatalogName = Path.GetFileNameWithoutExtension(file);

                    int matchedInFile = 0;
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        totalLinesProcessed++;

                        // 패턴 1: "cam/image_array"
                        var imgMatch = Regex.Match(line, @"""cam/image_array""\s*:\s*""([^""]+)""");

                        // 패턴 2: "image" (따옴표 포함)
                        if (!imgMatch.Success)
                        {
                            imgMatch = Regex.Match(line, @"""image""\s*:\s*""([^""]+)""");
                        }

                        // 패턴 3: image_array (따옴표 제거)
                        if (!imgMatch.Success)
                        {
                            imgMatch = Regex.Match(line, @"image_array\s*:\s*""([^""]+)""");
                        }

                        // 패턴 4: 단순 이미지 경로 (test_ 포함)
                        if (!imgMatch.Success)
                        {
                            imgMatch = Regex.Match(line, @"(test_[^""]*\.jpg)");
                        }

                        if (!imgMatch.Success) continue;

                        var angleMatch = Regex.Match(line, @"""user/angle""\s*:\s*([-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)");
                        if (!angleMatch.Success)
                        {
                            angleMatch = Regex.Match(line, @"user/angle["":]?\s*:\s*([-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)");
                        }

                        var throttleMatch = Regex.Match(line, @"""user/throttle""\s*:\s*([-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)");
                        if (!throttleMatch.Success)
                        {
                            throttleMatch = Regex.Match(line, @"user/throttle["":]?\s*:\s*([-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)");
                        }

                        FrameData fd = new FrameData();
                        fd.ImagePath = imgMatch.Groups[1].Value;
                        fd.CatalogName = currentCatalogName;  // 카탈로그 이름 설정
                        if (angleMatch.Success) fd.Angle = double.Parse(angleMatch.Groups[1].Value);
                        if (throttleMatch.Success) fd.Throttle = double.Parse(throttleMatch.Groups[1].Value);

                        frames.Add(fd);
                        framesLoaded++;
                        matchedInFile++;
                    }

                    AddLog($"   └─ 매칭된 프레임: {matchedInFile}개");
                }
                catch (Exception ex)
                {
                    AddLog($"⚠ 파일 로드 오류 ({Path.GetFileName(file)}): {ex.Message}");
                }
            }

            AddLog($"\n✓ 로드 완료: {framesLoaded}개 프레임 (총 {totalLinesProcessed}줄 처리)");

            if (frames.Count > 0)
            {
                // 원본 데이터 저장 (필터 초기화 시 사용)
                originalFrames = new List<FrameData>(frames);

                // 리스트박스에 먼저 아이템 추가
                for (int i = 0; i < frames.Count; i++)
                {
                    frames[i].FrameIndex = i;
                    lstFrames.Items.Add(frames[i]);
                }

                // 아이템 추가 후 트랙바 설정
                trackFrame.Minimum = 0;
                trackFrame.Maximum = frames.Count - 1;
                trackFrame.Value = 0;

                // 실제 이미지 파일 검증
                int missingImages = 0;
                int imagesFolder = 0;

                try
                {
                    string imagesPath = Path.Combine(folderPath, "images");
                    if (Directory.Exists(imagesPath))
                    {
                        imagesFolder = Directory.GetFiles(imagesPath, "*.jpg").Length;
                    }

                    // 누락된 이미지 확인
                    foreach (var frame in frames)
                    {
                        string imgPath = Path.Combine(folderPath, "images", frame.ImagePath);
                        if (!File.Exists(imgPath))
                        {
                            missingImages++;
                            if (missingImages <= 5)  // 처음 5개만 로그
                            {
                                AddLog($"⚠ 이미지 파일 없음: {frame.ImagePath}");
                            }
                        }
                    }

                    if (missingImages > 0)
                    {
                        AddLog($"⚠ 누락된 이미지: {missingImages}/{frames.Count}개");
                    }

                    AddLog($"📸 실제 이미지 파일: {imagesFolder}개");
                    AddLog($"📋 로드된 프레임: {frames.Count}개");
                    AddLog($"━━━━━━━━━━━━━━━━━");
                }
                catch (Exception ex)
                {
                    AddLog($"⚠ 이미지 검증 오류: {ex.Message}");
                }

                // 첫 이미지 자동 출력
                lstFrames.SelectedIndexChanged -= LstFrames_SelectedIndexChanged;
                trackFrame.Minimum = 0;
                trackFrame.Maximum = Math.Max(0, frames.Count - 1);
                if (lstFrames.Items.Count > 0 && frames.Count > 0)
                {
                    lstFrames.SelectedIndex = 0;
                    trackFrame.Value = 0;
                    ShowFrame(frames[0]);
                }
                lstFrames.SelectedIndexChanged += LstFrames_SelectedIndexChanged;
            }
            else
            {
                MessageBox.Show("지원되는 데이터가 없습니다.");
                AddLog("✗ 로드된 프레임 없음");
            }
        }

        private void LstFrames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstFrames.SelectedIndex >= 0 && lstFrames.SelectedIndex < frames.Count)
            {
                isUserInteracting = true;
                int idx = lstFrames.SelectedIndex;

                // 트랙바만 동기화 (다중 선택 시 마지막 선택만 표시)
                trackFrame.ValueChanged -= TrackFrame_Scroll;
                trackFrame.Value = idx;
                trackFrame.ValueChanged += TrackFrame_Scroll;

                // 이미지 표시
                ShowFrame(frames[idx]);
            }
        }

        private void TrackFrame_Scroll(object sender, EventArgs e)
        {
            if (trackFrame.Value >= 0 && trackFrame.Value < frames.Count)
            {
                isUserInteracting = true;

                // 리스트박스 이벤트 임시 제거
                lstFrames.SelectedIndexChanged -= LstFrames_SelectedIndexChanged;

                // 선택 해제 후 새로운 인덱스 선택
                lstFrames.ClearSelected();
                lstFrames.SelectedIndex = trackFrame.Value;

                // 이벤트 핸들러 복구
                lstFrames.SelectedIndexChanged += LstFrames_SelectedIndexChanged;

                // 이미지 표시
                ShowFrame(frames[trackFrame.Value]);
            }
        }

        private void ShowFrame(FrameData frame)
        {
            try
            {
                string imgPath = Path.Combine(currentFolder, "images", frame.ImagePath);
                if (!File.Exists(imgPath))
                {
                    imgPath = Path.Combine(currentFolder, frame.ImagePath);
                }

                if (File.Exists(imgPath))
                {
                    if (picMain.Image != null) picMain.Image.Dispose();
                    picMain.Image = Image.FromFile(imgPath);
                }

                _currentAngle = frame.Angle;
                _currentImageName = frame.ImagePath;
                picMain.Invalidate();

                lblAngleName.Text = "Angle: " + frame.Angle.ToString("F3");
                lblThrottleName.Text = "Throttle: " + frame.Throttle.ToString("F3");

                int angleVal = (int)((frame.Angle + 1.0) * 50.0);
                if (angleVal < 0) angleVal = 0;
                if (angleVal > 100) angleVal = 100;
                prgAngle.Value = angleVal;

                int throttleVal = (int)((frame.Throttle + 1.0) * 50.0);
                if (throttleVal < 0) throttleVal = 0;
                if (throttleVal > 100) throttleVal = 100;
                prgThrottle.Value = throttleVal;
            }
            catch (Exception ex)
            {
                txtLog.AppendText("이미지 불러오기 실패: " + ex.Message + "\n");
            }
        }

        private async void BtnAIReview_Click(object sender, EventArgs e)
        {
            // h5 파일 선택
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title  = "학습된 모델 선택";
                dlg.Filter = "Keras 모델 (*.h5)|*.h5|모든 파일 (*.*)|*.*";
                if (dlg.ShowDialog() != DialogResult.OK) return;
                _loadedModelPath = dlg.FileName;
            }

            if (string.IsNullOrEmpty(currentFolder))
            {
                MessageBox.Show("먼저 데이터 폴더를 불러와 주세요.", "알림");
                return;
            }

            string scriptPath = GetScriptPath("predict.py");
            if (scriptPath == null)
            {
                MessageBox.Show("predict.py 파일을 찾을 수 없습니다.");
                return;
            }

            _predictions.Clear();
            picMain.Invalidate();

            btnAIreview.Enabled = false;
            btnAIreview.Text = "예측 중...";
            AddLog($"AI 검증 시작: {Path.GetFileName(_loadedModelPath)}");

            var psi = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{scriptPath}\" --model \"{_loadedModelPath}\" --image_folder \"{currentFolder}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
                CreateNoWindow = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8,
                StandardErrorEncoding  = System.Text.Encoding.UTF8
            };

            int count = 0;
            await Task.Run(() =>
            {
                using (var proc = System.Diagnostics.Process.Start(psi))
                {
                    string line;
                    while ((line = proc.StandardOutput.ReadLine()) != null)
                    {
                        if (line.StartsWith("[PRED] "))
                        {
                            // [PRED] image=xxx.jpg angle=0.123456
                            try
                            {
                                var body  = line.Substring("[PRED] ".Length).Trim();
                                var parts = body.Split(' ');
                                string imgName = parts[0].Substring("image=".Length);
                                double angle   = double.Parse(parts[1].Substring("angle=".Length),
                                                    System.Globalization.CultureInfo.InvariantCulture);
                                lock (_predictions)
                                {
                                    _predictions[imgName] = angle;
                                }
                                count++;
                            }
                            catch { }
                        }
                    }
                    proc.WaitForExit();
                }
            });

            AddLog($"AI 검증 완료: {count}개 프레임 예측");
            btnAIreview.Enabled = true;
            btnAIreview.Text = "AI 검증";
            picMain.Invalidate();
        }

        private void PicMain_Paint(object sender, PaintEventArgs e)
        {
            if (picMain.Image == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int w = picMain.ClientSize.Width;
            int h = picMain.ClientSize.Height;

            // 화살표 기준점: 하단 중앙에서 위로 20px
            float cx = w / 2f;
            float cy = h - 20f;
            float lineLen = 90f;

            // angle(-1~1) → 각도(-20~+20도), 위쪽이 0도 기준
            double angleDeg = _currentAngle * 20.0;
            double angleRad = (angleDeg - 90.0) * Math.PI / 180.0;

            float tx = cx + (float)(lineLen * Math.Cos(angleRad));
            float ty = cy + (float)(lineLen * Math.Sin(angleRad));

            Color lineColor = Color.FromArgb(30, 120, 255);

            using (var pen = new Pen(lineColor, 4f))
            {
                g.DrawLine(pen, cx, cy, tx, ty);
            }

            // 파란 선 수치 텍스트 (실제 angle)
            string label = _currentAngle.ToString("F2");
            using (var font = new Font("Consolas", 10f, FontStyle.Bold))
            using (var brush = new SolidBrush(lineColor))
            {
                SizeF sz = g.MeasureString(label, font);
                g.DrawString(label, font, brush, cx - sz.Width / 2f - 18f, cy - lineLen - sz.Height - 2f);
            }

            // AI 예측 노란 선
            if (_currentImageName != null && _predictions.ContainsKey(_currentImageName))
            {
                double predAngle = _predictions[_currentImageName];
                double predDeg   = predAngle * 20.0;
                double predRad   = (predDeg - 90.0) * Math.PI / 180.0;

                float ptx = cx + (float)(lineLen * Math.Cos(predRad));
                float pty = cy + (float)(lineLen * Math.Sin(predRad));

                Color predColor = Color.FromArgb(255, 220, 0);
                using (var pen = new Pen(predColor, 4f))
                    g.DrawLine(pen, cx, cy, ptx, pty);

                // 노란 선 수치 텍스트 (예측 angle)
                string predLabel = predAngle.ToString("F2");
                using (var font = new Font("Consolas", 10f, FontStyle.Bold))
                using (var brush = new SolidBrush(predColor))
                {
                    SizeF sz = g.MeasureString(predLabel, font);
                    g.DrawString(predLabel, font, brush, cx - sz.Width / 2f + 18f, cy - lineLen - sz.Height - 2f);
                }
            }
        }

        private string GetScriptPath(string scriptName)
        {
            string exeDir = Path.GetDirectoryName(Application.ExecutablePath);
            string scriptInBinDir = Path.Combine(exeDir, scriptName);
            if (File.Exists(scriptInBinDir)) return scriptInBinDir;

            string projectRoot = Path.GetDirectoryName(Path.GetDirectoryName(exeDir));
            string scriptInRoot = Path.Combine(projectRoot, scriptName);
            if (File.Exists(scriptInRoot)) return scriptInRoot;

            return null;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void lblCount_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void BtnRestore_Click(object sender, EventArgs e)
        {
            string deletedFolder = Path.Combine(currentFolder, "_deleted");

            if (!Directory.Exists(deletedFolder))
            {
                MessageBox.Show("복원할 데이터가 없습니다.\n(_deleted 폴더 없음)", "알림");
                return;
            }

            var result = MessageBox.Show(
                "_deleted 폴더의 모든 데이터를 복원하시겠습니까?",
                "복원 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            try
            {
                int restoredImages = 0;
                int restoredRecords = 0;

                // 1. 이미지 복원
                string deletedImages = Path.Combine(deletedFolder, "images");
                if (Directory.Exists(deletedImages))
                {
                    foreach (string imgFile in Directory.GetFiles(deletedImages, "*.jpg"))
                    {
                        string dest = Path.Combine(currentFolder, "images", Path.GetFileName(imgFile));
                        if (!File.Exists(dest))
                        {
                            File.Move(imgFile, dest);
                            restoredImages++;
                        }
                    }
                }

                // 2. catalog 복원 (index 기준 정렬하여 원래 순서 유지)
                foreach (string backupCatalog in Directory.GetFiles(deletedFolder, "*.catalog"))
                {
                    string originalCatalog = Path.Combine(currentFolder, Path.GetFileName(backupCatalog));
                    if (File.Exists(originalCatalog))
                    {
                        var linesToRestore = File.ReadAllLines(backupCatalog, Encoding.UTF8)
                            .Where(l => !string.IsNullOrWhiteSpace(l)).ToList();

                        // 기존 라인 + 복원 라인 합치기
                        var allLines = File.ReadAllLines(originalCatalog, Encoding.UTF8)
                            .Where(l => !string.IsNullOrWhiteSpace(l))
                            .Concat(linesToRestore)
                            .ToList();

                        // index 필드 기준으로 정렬
                        var sorted = allLines
                            .Select(l => {
                                int idx = int.MaxValue;
                                try {
                                    var m = System.Text.RegularExpressions.Regex.Match(l, @"""_index""\s*:\s*(\d+)");
                                    if (!m.Success)
                                        m = System.Text.RegularExpressions.Regex.Match(l, @"""index""\s*:\s*(\d+)");
                                    if (m.Success) idx = int.Parse(m.Groups[1].Value);
                                } catch { }
                                return new { Line = l, Idx = idx };
                            })
                            .OrderBy(x => x.Idx)
                            .Select(x => x.Line)
                            .ToArray();

                        File.WriteAllLines(originalCatalog, sorted, Encoding.UTF8);
                        restoredRecords += linesToRestore.Count;
                    }
                    File.Delete(backupCatalog);
                }

                // 3. _deleted 폴더 정리
                if (Directory.Exists(deletedImages) &&
                    Directory.GetFiles(deletedImages).Length == 0)
                    Directory.Delete(deletedImages);
                if (Directory.GetFiles(deletedFolder).Length == 0 &&
                    Directory.GetDirectories(deletedFolder).Length == 0)
                    Directory.Delete(deletedFolder);

                AddLog($"✅ 복원 완료: 이미지 {restoredImages}개, 레코드 {restoredRecords}개");
                MessageBox.Show(
                    $"복원 완료!\n이미지: {restoredImages}개\n레코드: {restoredRecords}개",
                    "복원 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 폴더 자동 재로드
                LoadCatalog(currentFolder);
                if (_graphWindow != null && !_graphWindow.IsDisposed)
                    _graphWindow.LoadFromFolder(currentFolder, frames);
            }
            catch (Exception ex)
            {
                AddLog($"❌ 복원 실패: {ex.Message}");
                MessageBox.Show($"복원 중 오류 발생:\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string PrepareTrainingFolder()
        {
            string baseFolder = txtPath.Text.Trim();
            if (!Directory.Exists(baseFolder))
            {
                MessageBox.Show("폴더가 존재하지 않습니다.");
                return null;
            }

            // 필터링 안된 상태면 원본 폴더 그대로 사용
            if (frames.Count == originalFrames.Count)
            {
                AddLog("필터링 없음 - 전체 데이터로 학습합니다.");
                return baseFolder;
            }

            // 임시 학습 폴더 생성
            string tempFolder = Path.Combine(baseFolder, "_train_temp");
            string tempImages = Path.Combine(tempFolder, "images");

            try
            {
                // 기존 임시 폴더 삭제 후 재생성
                if (Directory.Exists(tempFolder))
                    Directory.Delete(tempFolder, true);
                Directory.CreateDirectory(tempImages);

                // 필터링된 프레임의 이미지만 복사
                int copied = 0;
                foreach (var frame in frames)
                {
                    string src = Path.Combine(baseFolder, "images", frame.ImagePath);
                    string dst = Path.Combine(tempImages, frame.ImagePath);
                    if (File.Exists(src))
                    {
                        File.Copy(src, dst, true);
                        copied++;
                    }
                }

                // 필터링된 프레임으로 임시 catalog 파일 생성
                string tempCatalog = Path.Combine(tempFolder, "catalog_filtered.catalog");
                var lines = new List<string>();

                // 원본 catalog에서 필터링된 프레임 라인만 추출
                var validImages = new HashSet<string>(frames.Select(f => f.ImagePath));
                foreach (var file in Directory.GetFiles(baseFolder, "*.catalog")
                    .Where(f => !f.EndsWith(".catalog_manifest")))
                {
                    foreach (var line in File.ReadAllLines(file, System.Text.Encoding.UTF8))
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        // 해당 라인이 필터링된 프레임 중 하나인지 확인
                        if (validImages.Any(img => line.Contains(img)))
                            lines.Add(line);
                    }
                }

                File.WriteAllLines(tempCatalog, lines, System.Text.Encoding.UTF8);

                AddLog($"✅ 필터링된 데이터 준비 완료: {copied}개 이미지 / {lines.Count}개 레코드");
                return tempFolder;
            }
            catch (Exception ex)
            {
                AddLog($"❌ 임시 폴더 생성 실패: {ex.Message}");
                if (Directory.Exists(tempFolder))
                    Directory.Delete(tempFolder, true);
                return null;
            }
        }

        /// <summary>
        /// 범위 선택 버튼 클릭 이벤트
        /// 첫 클릭: 시작 프레임 저장
        /// 두 번째 클릭: 범위 내 프레임 모두 선택
        /// </summary>
        private void BtnRangeSelect_Click(object sender, EventArgs e)
        {
            // 재생 중에는 선택 목록이 고정될 수 있으므로 실제 현재 위치는 트랙바 기준으로 잡는다.
            int currentIdx = GetCurrentFrameIndex();

            if (currentIdx < 0)
            {
                MessageBox.Show("먼저 프레임을 선택해주세요.", "알림");
                return;
            }

            if (isSelectingRangeStart)
            {
                // 첫 번째 클릭: 시작 프레임 저장
                rangeStartIndex = currentIdx;
                btnRangeSelect.Text = "끝점 선택";
                btnRangeSelect.BackColor = Color.FromArgb(200, 140, 0);
                isSelectingRangeStart = false;
                AddLog($"📍 범위 시작: {rangeStartIndex}번 프레임");
            }
            else
            {
                // 두 번째 클릭: 범위 선택 완료
                int rangeEndIndex = currentIdx;

                int minIndex = Math.Min(rangeStartIndex, rangeEndIndex);
                int maxIndex = Math.Max(rangeStartIndex, rangeEndIndex);

                // 범위 내 모든 항목 선택
                lstFrames.SelectedIndexChanged -= LstFrames_SelectedIndexChanged;
                for (int i = minIndex; i <= maxIndex; i++)
                    lstFrames.SetSelected(i, true);
                lstFrames.SelectedIndexChanged += LstFrames_SelectedIndexChanged;

                // UI 업데이트
                UpdateRangeSelectionUI(minIndex, maxIndex);
                AddDeleteRange(minIndex, maxIndex);
                lstFrames.TopIndex = minIndex;

                // 버튼 초기화 (다음 범위 선택 바로 가능)
                ResetRangeSelectionState();

                AddLog($"✓ 범위 선택 완료: {minIndex}~{maxIndex}번 ({maxIndex - minIndex + 1}개)");
            }
        }

        private void AddDeleteRange(int startIndex, int endIndex)
        {
            int start = Math.Max(0, Math.Min(startIndex, endIndex));
            int end = Math.Min(frames.Count - 1, Math.Max(startIndex, endIndex));
            if (start > end) return;

            DeleteRange range = new DeleteRange { StartIndex = start, EndIndex = end };
            deleteRanges.Add(range);

            lstDeleteRanges.Items.Add(range);
            lstDeleteRanges.SetSelected(lstDeleteRanges.Items.Count - 1, true);
        }

        private void UpdateDeleteRangesAfterDeletion(List<int> deletedIndicesDescending, List<int> deletedRangeIndices)
        {
            if (lstDeleteRanges == null) return;

            var deletedIndices = deletedIndicesDescending
                .Distinct()
                .OrderBy(i => i)
                .ToList();
            var deletedIndexSet = new HashSet<int>(deletedIndices);
            var deletedRangeSet = new HashSet<int>(deletedRangeIndices);
            var rebuiltRanges = new List<DeleteRange>();

            for (int rangeIndex = 0; rangeIndex < deleteRanges.Count; rangeIndex++)
            {
                if (deletedRangeSet.Contains(rangeIndex))
                    continue;

                DeleteRange range = deleteRanges[rangeIndex];
                var mappedIndices = new List<int>();

                for (int oldIndex = range.StartIndex; oldIndex <= range.EndIndex; oldIndex++)
                {
                    if (deletedIndexSet.Contains(oldIndex))
                        continue;

                    int newIndex = oldIndex - CountDeletedBefore(deletedIndices, oldIndex);
                    if (newIndex >= 0 && newIndex < frames.Count)
                        mappedIndices.Add(newIndex);
                }

                AddContiguousRanges(mappedIndices, rebuiltRanges);
            }

            deleteRanges.Clear();
            deleteRanges.AddRange(rebuiltRanges);

            lstDeleteRanges.Items.Clear();
            foreach (DeleteRange range in deleteRanges)
                lstDeleteRanges.Items.Add(range);

            for (int i = 0; i < lstDeleteRanges.Items.Count; i++)
                lstDeleteRanges.SetSelected(i, true);
        }

        private int CountDeletedBefore(List<int> deletedIndicesAscending, int index)
        {
            int count = 0;
            foreach (int deletedIndex in deletedIndicesAscending)
            {
                if (deletedIndex >= index)
                    break;
                count++;
            }
            return count;
        }

        private void AddContiguousRanges(List<int> indices, List<DeleteRange> target)
        {
            if (indices.Count == 0) return;

            indices.Sort();
            int start = indices[0];
            int end = indices[0];

            for (int i = 1; i < indices.Count; i++)
            {
                if (indices[i] == end + 1)
                {
                    end = indices[i];
                    continue;
                }

                target.Add(new DeleteRange { StartIndex = start, EndIndex = end });
                start = indices[i];
                end = indices[i];
            }

            target.Add(new DeleteRange { StartIndex = start, EndIndex = end });
        }

        private IEnumerable<int> GetIndicesSelectedForDeletion()
        {
            if (lstDeleteRanges.SelectedIndices.Count > 0)
            {
                foreach (int rangeIndex in lstDeleteRanges.SelectedIndices)
                {
                    if (rangeIndex < 0 || rangeIndex >= deleteRanges.Count)
                        continue;

                    DeleteRange range = deleteRanges[rangeIndex];
                    int start = Math.Max(0, range.StartIndex);
                    int end = Math.Min(frames.Count - 1, range.EndIndex);
                    for (int i = start; i <= end; i++)
                        yield return i;
                }

                yield break;
            }

            foreach (int index in lstFrames.SelectedIndices)
                yield return index;
        }

        private void ResetRangeSelectionState(bool clearRangeInfo = false)
        {
            btnRangeSelect.Text = "범위 선택";
            btnRangeSelect.BackColor = Color.FromArgb(0, 150, 136);
            isSelectingRangeStart = true;
            rangeStartIndex = -1;

            if (!clearRangeInfo) return;

            deleteRanges.Clear();
            if (lstDeleteRanges != null)
                lstDeleteRanges.Items.Clear();
        }

        /// <summary>
        /// 범위 선택 UI 업데이트
        /// 선택된 개수, 시작/끝 프레임 번호, 라벨 텍스트 업데이트
        /// </summary>
        private void UpdateRangeSelectionUI(int startIndex, int endIndex)
        {
            try
            {
                int selectedCount = lstFrames.SelectedIndices.Count;

                AddLog($"📊 범위 정보: 시작={startIndex:D6}, 끝={endIndex:D6}, 개수={selectedCount}개");
            }
            catch (Exception ex)
            {
                AddLog($"⚠ 범위 선택 UI 업데이트 오류: {ex.Message}");
            }
        }
    }

    public class FrameData
    {
        public int FrameIndex { get; set; }
        public string ImagePath { get; set; }
        public double Angle { get; set; }
        public double Throttle { get; set; }
        public string CatalogName { get; set; }  // 카탈로그 이름 (예: catalog_0)

        public override string ToString()
        {
            return $"[{FrameIndex}] {ImagePath}";
        }
    }
}
