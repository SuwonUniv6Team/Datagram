using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        private string currentFolder = "";
        private Timer playbackTimer;
        private float playbackSpeed = 1.0f;
        private bool isUserInteracting = false;

        public Form1()
        {
            InitializeComponent();
            btnLoad.Click += BtnLoad_Click;
            lstFrames.SelectedIndexChanged += LstFrames_SelectedIndexChanged;
            trackFrame.Scroll += TrackFrame_Scroll;

            // 재생 제어 버튼 이벤트 등록
            btnplay.Click += BtnPlay_Click;
            btnpurse.Click += BtnPause_Click;
            btnnext.Click += BtnNext_Click;
            btntrace.Click += BtnTrace_Click;
            btnFilter.Click += BtnFilter_Click;
            btnDelete.Click += BtnDelete_Click;
            btnTrain.Click += BtnTrain_Click;

            // 단일 선택 모드 (재생 중 프레임 이동을 위해 필수)
            lstFrames.SelectionMode = SelectionMode.One;

            // Timer 초기화 (약 10 FPS 설정)
            playbackTimer = new Timer();
            playbackTimer.Interval = 100; // 100ms 마다 틱
            playbackTimer.Tick += PlaybackTimer_Tick;

            // 배속 콤보박스 초기화
            InitializeSpeedComboBox();
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

        private void BtnTrain_Click(object sender, EventArgs e)
        {
            AddLog("AI 학습 시작");
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
                lstFrames.ClearSelected();
                lstFrames.SelectedIndex = 0;
            }

            AddLog("자동재생 시작");
            playbackTimer.Start();
        }

        private void BtnPause_Click(object sender, EventArgs e)
        {
            AddLog("자동재생 정지");
            playbackTimer.Stop();
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            playbackTimer.Stop(); // 수동 이동시 재생 멈춤
            if (frames.Count > 0 && lstFrames.SelectedIndex < frames.Count - 1)
            {
                int nextIdx = lstFrames.SelectedIndex < 0 ? 0 : lstFrames.SelectedIndex + 1;
                lstFrames.SelectedIndexChanged -= LstFrames_SelectedIndexChanged;
                lstFrames.SelectedIndex = nextIdx;
                lstFrames.SelectedIndexChanged += LstFrames_SelectedIndexChanged;
                ShowFrame(frames[nextIdx]);
                AddLog($"프레임 이동: {nextIdx}번");
            }
        }

        private void BtnTrace_Click(object sender, EventArgs e)
        {
            playbackTimer.Stop(); // 수동 이동시 재생 멈춤
            if (frames.Count > 0 && lstFrames.SelectedIndex > 0)
            {
                int prevIdx = lstFrames.SelectedIndex - 1;
                lstFrames.SelectedIndexChanged -= LstFrames_SelectedIndexChanged;
                lstFrames.SelectedIndex = prevIdx;
                lstFrames.SelectedIndexChanged += LstFrames_SelectedIndexChanged;
                ShowFrame(frames[prevIdx]);
                AddLog($"프레임 이동: {prevIdx}번");
            }
        }

        private void PlaybackTimer_Tick(object sender, EventArgs e)
        {
            if (frames.Count == 0)
            {
                playbackTimer.Stop();
                return;
            }

            // 사용자가 상호작용 중이면 다음 틱 때까지 대기
            if (isUserInteracting)
            {
                isUserInteracting = false;
                return;
            }

            int currentIdx = lstFrames.SelectedIndex;
            if (currentIdx < 0) currentIdx = 0;

            int nextIdx = currentIdx + (int)playbackSpeed;

            if (nextIdx < frames.Count)
            {
                lstFrames.SelectedIndexChanged -= LstFrames_SelectedIndexChanged;
                lstFrames.SelectedIndex = nextIdx;
                lstFrames.SelectedIndexChanged += LstFrames_SelectedIndexChanged;
                ShowFrame(frames[nextIdx]);
            }
            else
            {
                playbackTimer.Stop(); // 마지막 프레임에 도달하면 정지
            }
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            if (frames == null || frames.Count == 0) return;

            int beforeCount = frames.Count;

            // Throttle <= 0.05 인 프레임 제거
            frames = frames.Where(f => f.Throttle > 0.05).ToList();

            int afterCount = frames.Count;

            // 리스트 및 프레임 번호 갱신
            lstFrames.Items.Clear();
            for (int i = 0; i < frames.Count; i++)
            {
                frames[i].FrameIndex = i;
                lstFrames.Items.Add(frames[i]);
            }

            // 트랙바 및 화면 갱신
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

            AddLog($"데이터 필터링 완료: {beforeCount}개 -> {afterCount}개");
            MessageBox.Show($"필터링 전: {beforeCount}개\n필터링 후: {afterCount}개", "필터링 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (lstFrames.SelectedIndices.Count == 0 || frames.Count == 0) return;

            var result = MessageBox.Show("현재 프레임을 삭제하시겠습니까?", "프레임 삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // 인덱스가 꼬이지 않도록 내림차순 정렬 후 삭제
                var selectedIndices = lstFrames.SelectedIndices.Cast<int>().OrderByDescending(i => i).ToList();
                int nextIndex = selectedIndices.Min(); // 삭제할 가장 첫 번째 인덱스 저장

                foreach (int idx in selectedIndices)
                {
                    frames.RemoveAt(idx);
                }

                string deletedStr = string.Join(", ", selectedIndices);
                AddLog($"프레임 삭제 완료: {deletedStr}번");

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

                    lstFrames.SelectedIndex = nextIndex;
                    trackFrame.Value = nextIndex;
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
                return;
            }

            foreach (var file in files)
            {
                string[] lines = File.ReadAllLines(file);
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var imgMatch = Regex.Match(line, @"""cam/image_array""\s*:\s*""([^""]+)""");
                    if (!imgMatch.Success) continue;

                    var angleMatch = Regex.Match(line, @"""user/angle""\s*:\s*([-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)");
                    var throttleMatch = Regex.Match(line, @"""user/throttle""\s*:\s*([-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)");

                    FrameData fd = new FrameData();
                    fd.ImagePath = imgMatch.Groups[1].Value;
                    if (angleMatch.Success) fd.Angle = double.Parse(angleMatch.Groups[1].Value);
                    if (throttleMatch.Success) fd.Throttle = double.Parse(throttleMatch.Groups[1].Value);

                    frames.Add(fd);
                }
            }

            if (frames.Count > 0)
            {
                trackFrame.Minimum = 0;
                trackFrame.Maximum = frames.Count - 1;
                trackFrame.Value = 0;

                for (int i = 0; i < frames.Count; i++)
                {
                    frames[i].FrameIndex = i;
                    lstFrames.Items.Add(frames[i]);
                }

                AddLog($"데이터 로드 완료: {frames.Count}개");
                // 첫 이미지 자동 출력
                lstFrames.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("지원되는 데이터가 없습니다.");
            }
        }

        private void LstFrames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstFrames.SelectedIndex >= 0 && lstFrames.SelectedIndex < frames.Count)
            {
                isUserInteracting = true;
                int idx = lstFrames.SelectedIndex;
                trackFrame.ValueChanged -= TrackFrame_Scroll;
                trackFrame.Value = idx;
                trackFrame.ValueChanged += TrackFrame_Scroll;
                ShowFrame(frames[idx]);
            }
        }

        private void TrackFrame_Scroll(object sender, EventArgs e)
        {
            if (trackFrame.Value >= 0 && trackFrame.Value < frames.Count)
            {
                isUserInteracting = true;
                lstFrames.SelectedIndexChanged -= LstFrames_SelectedIndexChanged;
                lstFrames.SelectedIndex = trackFrame.Value;
                lstFrames.SelectedIndexChanged += LstFrames_SelectedIndexChanged;
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
    }

    public class FrameData
    {
        public int FrameIndex { get; set; }
        public string ImagePath { get; set; }
        public double Angle { get; set; }
        public double Throttle { get; set; }

        public override string ToString()
        {
            return $"[{FrameIndex}] {ImagePath}";
        }
    }
}
