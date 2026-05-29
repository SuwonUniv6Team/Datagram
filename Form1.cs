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

        public Form1()
        {
            InitializeComponent();
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

            // 다중 선택 모드 활성화 (Ctrl/Shift + 클릭으로 다중 선택 가능)
            lstFrames.SelectionMode = SelectionMode.MultiExtended;

            // Timer 초기화 (약 10 FPS 설정)
            playbackTimer = new Timer();
            playbackTimer.Interval = 100; // 100ms 마다 틱
            playbackTimer.Tick += PlaybackTimer_Tick;

            // 배속 콤보박스 초기화
            InitializeSpeedComboBox();

            // 필터 초기화 버튼 이벤트 등록
            btnreset.Click += BtnResetFilter_Click;

            // nud1 (급커브 수치) 초기화 - 소수점 0.1씩 증가/감소
            nud1.Increment = (decimal)0.1;
            nud1.DecimalPlaces = 1;
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

        private async void BtnTrain_Click(object sender, EventArgs e)
        {
            string imageFolder = txtPath.Text.Trim();

            if (!Directory.Exists(imageFolder))
            {
                MessageBox.Show("폴더가 존재하지 않습니다.");
                return;
            }

            // 버튼 비활성화 (중복 클릭 방지)
            btnTrain.Enabled = false;
            AddLog("환경 확인 중...");

            // 백그라운드에서 설치 확인 및 설치 진행
            bool ready = await Task.Run(() =>
            {
                var starter = new TrainStarter(txtLog);
                return starter.EnsureReady();
            });

            if (!ready)
            {
                AddLog("❌ 환경 준비 실패. 학습을 시작할 수 없습니다.");
                btnTrain.Enabled = true;
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
                Arguments = $"\"{scriptPath}\" --image_folder \"{imageFolder}\"",
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
                if (!string.IsNullOrEmpty(args.Data))
                    Invoke(new Action(() => txtLog.AppendText(args.Data + Environment.NewLine)));
            };

            process.ErrorDataReceived += (s, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                    Invoke(new Action(() => txtLog.AppendText("[ERROR] " + args.Data + Environment.NewLine)));
            };

            process.Exited += (s, args) =>
            {
                Invoke(new Action(() => btnTrain.Enabled = true));
            };

            process.Start();
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

        private void BtnPlay_Click(object sender, EventArgs e)
        {
            if (frames.Count == 0) return;

            // 이미 마지막 프레임인 경우 처음부터 다시 재생
            if (lstFrames.SelectedIndex >= frames.Count - 1)
            {
                lstFrames.SelectedIndexChanged -= LstFrames_SelectedIndexChanged;
                lstFrames.ClearSelected();
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

        private void BtnNext_Click(object sender, EventArgs e)
        {
            playbackTimer.Stop();
            isPlaybackActive = false;

            if (frames.Count == 0) return;

            int currentIdx = lstFrames.SelectedIndex;
            if (currentIdx < 0) currentIdx = 0;

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

            int currentIdx = lstFrames.SelectedIndex;
            if (currentIdx < 0) currentIdx = frames.Count - 1;

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
                int currentIdx = lstFrames.SelectedIndex;
                if (currentIdx < 0) currentIdx = 0;

                int nextIdx = currentIdx + (int)playbackSpeed;

                if (nextIdx < frames.Count)
                {
                    // 이벤트 핸들러 임시 제거
                    lstFrames.SelectedIndexChanged -= LstFrames_SelectedIndexChanged;

                    // 모든 선택 해제 후 현재 프레임만 선택
                    lstFrames.ClearSelected();
                    lstFrames.SelectedIndex = nextIdx;

                    // 이벤트 핸들러 복구
                    lstFrames.SelectedIndexChanged += LstFrames_SelectedIndexChanged;

                    // 이미지 표시
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
                    trackFrame.Value = 0;
                    lstFrames.SelectedIndex = 0;
                }
                else
                {
                    trackFrame.Maximum = 0;
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
                    trackFrame.Value = 0;
                    lstFrames.SelectedIndex = 0;
                }
                else
                {
                    trackFrame.Maximum = 0;
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
            if (lstFrames.SelectedIndices.Count == 0 || frames.Count == 0) return;

            var result = MessageBox.Show("현재 프레임을 삭제하시겠습니까?\n(이미지와 데이터가 완전히 삭제됩니다)", 
                "프레임 삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // 0. 먼저 모든 이미지를 메모리에서 확실하게 해제
                AddLog("🔓 메모리에서 모든 이미지 해제 중...");
                try
                {
                    if (picMain.Image != null)
                    {
                        picMain.Image.Dispose();
                        picMain.Image = null;
                    }

                    // 가비지 컬렉션 3회 실행 (확실한 메모리 해제)
                    for (int i = 0; i < 3; i++)
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }

                    System.Threading.Thread.Sleep(200); // 200ms 대기
                    AddLog("✓ 메모리 정리 완료");
                }
                catch (Exception ex)
                {
                    AddLog($"⚠ 메모리 해제 중 오류: {ex.Message}");
                }

                // 인덱스가 꼬이지 않도록 내림차순 정렬 후 삭제
                var selectedIndices = lstFrames.SelectedIndices.Cast<int>().OrderByDescending(i => i).ToList();
                int nextIndex = selectedIndices.Min(); // 삭제할 가장 첫 번째 인덱스 저장

                int imageDeletedCount = 0;
                int recordDeletedCount = 0;
                int imageDeletionFailedCount = 0;

                // 선택된 프레임 데이터와 이미지 파일 삭제
                foreach (int idx in selectedIndices)
                {
                    try
                    {
                        // 1. 이미지 파일 삭제
                        if (DeleteImageFile(frames[idx]))
                        {
                            imageDeletedCount++;
                        }
                        else
                        {
                            imageDeletionFailedCount++;
                        }

                        // 2. Catalog/Record 파일에서 데이터 제거
                        if (DeleteRecordData(frames[idx]))
                        {
                            recordDeletedCount++;
                        }

                        frames.RemoveAt(idx);
                    }
                    catch (Exception ex)
                    {
                        AddLog($"프레임 {idx} 삭제 실패: {ex.Message}");
                    }
                }

                // 삭제 결과 메시지
                string deletedStr = string.Join(", ", selectedIndices);
                AddLog($"━━━ 프레임 삭제 결과 ━━━");
                AddLog($"  삭제 인덱스: {deletedStr}");
                AddLog($"  ✓ 이미지 삭제: {imageDeletedCount}개");
                if (imageDeletionFailedCount > 0)
                {
                    AddLog($"  ✗ 이미지 삭제 실패: {imageDeletionFailedCount}개");
                    AddLog($"  💡 팁: OneDrive 또는 클라우드 동기화 폴더를 사용 중이면 동기화가 완료될 때까지 대기하세요.");
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
                    trackFrame.Maximum = frames.Count - 1;

                    // 다음 프레임 인덱스 보정
                    if (nextIndex >= frames.Count)
                    {
                        nextIndex = frames.Count - 1;
                    }

                    lstFrames.SelectedIndexChanged -= LstFrames_SelectedIndexChanged;
                    lstFrames.SelectedIndex = nextIndex;
                    lstFrames.SelectedIndexChanged += LstFrames_SelectedIndexChanged;
                    trackFrame.Value = nextIndex;
                    ShowFrame(frames[nextIndex]);
                }
                else
                {
                    // 데이터가 0개가 된 경우 초기화
                    trackFrame.Maximum = 0;
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

                // 원본 데이터도 동기화 (필터 초기화 시 삭제된 프레임이 다시 나타나는 것을 방지)
                originalFrames = new List<FrameData>(frames);
            }
        }

        /// <summary>
        /// 이미지 파일 삭제 (강화된 버전 - 여러 번 재시도 포함)
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

                AddLog($"🗑️  파일 삭제 시도: {frame.ImagePath}");

                // 메모리에서 이미지 해제 (모든 이미지, 캐시 포함)
                AddLog($"🔓 메모리에서 이미지 해제 중...");
                if (picMain.Image != null)
                {
                    try
                    {
                        picMain.Image.Dispose();
                        picMain.Image = null;
                    }
                    catch { }
                }

                // 가비지 컬렉션 강제 실행
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                System.Threading.Thread.Sleep(100);  // 100ms 대기

                // 파일 삭제 시도 (최대 5회)
                int maxRetries = 5;
                int retryCount = 0;
                bool deleted = false;

                while (!deleted && retryCount < maxRetries)
                {
                    try
                    {
                        File.Delete(imgPath);
                        deleted = true;
                        AddLog($"✓ 이미지 삭제 성공: {frame.ImagePath}");
                        return true;
                    }
                    catch (IOException ioEx)
                    {
                        retryCount++;
                        if (retryCount < maxRetries)
                        {
                            AddLog($"⚠ 재시도 {retryCount}/{maxRetries-1} - {ioEx.Message}");
                            System.Threading.Thread.Sleep(300 * retryCount);  // 점진적 대기 (300ms, 600ms, 900ms...)
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
                AddLog($"   경로: {imgPath}");
                return false;
            }
            catch (IOException ioEx)
            {
                AddLog($"✗ 파일 잠금 해제 불가: {ioEx.Message}");
                AddLog($"   경로: {imgPath}");
                AddLog($"   파일을 다른 프로그램에서 사용 중일 수 있습니다.");
                return false;
            }
            catch (Exception ex)
            {
                AddLog($"✗ 이미지 삭제 실패: {ex.Message}");
                AddLog($"   경로: {imgPath}");
                return false;
            }
        }

        /// <summary>
        /// Catalog/Record 파일에서 프레임 데이터 제거
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
        /// JSON 파일에서 프레임 데이터 제거
        /// </summary>
        private bool DeleteFromJsonFile(string jsonPath, FrameData frame)
        {
            try
            {
                string content = File.ReadAllText(jsonPath);
                string imageFileName = Path.GetFileName(frame.ImagePath);

                AddLog($"📄 JSON 파일 검사: {Path.GetFileName(jsonPath)}");
                AddLog($"   찾는 이미지: {imageFileName}");

                // JSON 파일 내용에서 이미지 경로 찾기
                if (!content.Contains(imageFileName))
                {
                    AddLog($"   ⓘ 해당 이미지를 찾을 수 없음");
                    return false;
                }

                // 라인 단위로 필터링
                var lines = File.ReadAllLines(jsonPath);
                var originalLineCount = lines.Length;

                // 이미지 파일명을 포함한 라인 제거
                var filteredLines = lines.Where(line => !line.Contains(imageFileName)).ToArray();

                if (originalLineCount != filteredLines.Length)
                {
                    // JSON 구조 보정 (쉼표 처리)
                    filteredLines = FixJsonStructure(filteredLines);

                    File.WriteAllLines(jsonPath, filteredLines, Encoding.UTF8);
                    AddLog($"✓ JSON에서 {originalLineCount - filteredLines.Length}개 라인 제거");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                AddLog($"✗ JSON 파일 처리 오류: {ex.Message}");
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
        /// 텍스트 형식 Catalog 파일에서 프레임 데이터 제거
        /// </summary>
        private bool DeleteFromTextFile(string catalogPath, FrameData frame)
        {
            try
            {
                string imageFileName = Path.GetFileName(frame.ImagePath);

                AddLog($"📄 텍스트 파일 검사: {Path.GetFileName(catalogPath)}");
                AddLog($"   찾는 이미지: {imageFileName}");

                string[] lines = File.ReadAllLines(catalogPath);
                var originalLineCount = lines.Length;

                // 이미지 파일명을 포함한 라인 제거
                string[] filteredLines = lines.Where(line => 
                    !line.Contains(imageFileName) && 
                    !line.Contains(frame.ImagePath)
                ).ToArray();

                if (originalLineCount != filteredLines.Length)
                {
                    File.WriteAllLines(catalogPath, filteredLines, Encoding.UTF8);
                    AddLog($"✓ 텍스트에서 {originalLineCount - filteredLines.Length}개 라인 제거");
                    return true;
                }
                else
                {
                    AddLog($"   ⓘ 해당 이미지를 찾을 수 없음");
                }

                return false;
            }
            catch (Exception ex)
            {
                AddLog($"✗ 텍스트 파일 처리 오류: {ex.Message}");
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
                }
            }
        }

        private void LoadCatalog(string folderPath)
        {
            frames.Clear();
            lstFrames.Items.Clear();
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

                trackFrame.Minimum = 0;
                trackFrame.Maximum = frames.Count - 1;
                trackFrame.Value = 0;

                for (int i = 0; i < frames.Count; i++)
                {
                    frames[i].FrameIndex = i;
                    lstFrames.Items.Add(frames[i]);
                }

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
                lstFrames.SelectedIndex = 0;
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
