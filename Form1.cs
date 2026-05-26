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
        private string currentFolder = "";
        private Timer playbackTimer;

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

            // 다중 선택 지원
            lstFrames.SelectionMode = SelectionMode.MultiExtended;

            // Timer 초기화 (약 10 FPS 설정)
            playbackTimer = new Timer();
            playbackTimer.Interval = 100; // 100ms 마다 틱
            playbackTimer.Tick += PlaybackTimer_Tick;
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
                lstFrames.ClearSelected();
                lstFrames.SelectedIndex = nextIdx;
                AddLog($"프레임 이동: {nextIdx}번");
            }
        }

        private void BtnTrace_Click(object sender, EventArgs e)
        {
            playbackTimer.Stop(); // 수동 이동시 재생 멈춤
            if (frames.Count > 0 && lstFrames.SelectedIndex > 0)
            {
                int prevIdx = lstFrames.SelectedIndex - 1;
                lstFrames.ClearSelected();
                lstFrames.SelectedIndex = prevIdx;
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

            int currentIdx = lstFrames.SelectedIndex;
            if (currentIdx < 0) currentIdx = 0;

            if (currentIdx < frames.Count - 1)
            {
                lstFrames.ClearSelected();
                lstFrames.SelectedIndex = currentIdx + 1;
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

            try
            {
                // 1단계: .catalog 또는 .catalog_manifest 파일 찾기
                var catalogFiles = Directory.GetFiles(folderPath, "*.catalog", SearchOption.AllDirectories)
                    .Union(Directory.GetFiles(folderPath, "*.catalog_manifest", SearchOption.AllDirectories))
                    .Distinct()
                    .ToList();

                // 2단계: record_xxx.json 파일도 찾기 (tub 폴더)
                var recordFiles = Directory.GetFiles(folderPath, "record_*.json", SearchOption.AllDirectories)
                    .ToList();

                // 3단계: catalog 파일이 있으면 파싱
                if (catalogFiles.Count > 0)
                {
                    foreach (var catalogFile in catalogFiles)
                    {
                        LoadFramesFromCatalog(catalogFile, folderPath);
                    }
                }

                // 4단계: record_*.json 파일이 없거나, catalog에서 프레임을 찾지 못한 경우에만 파싱
                if (recordFiles.Count > 0 && frames.Count == 0)
                {
                    foreach (var recordFile in recordFiles)
                    {
                        LoadFramesFromJson(recordFile, Path.GetDirectoryName(recordFile));
                    }
                }

                // 5단계: 데이터가 없으면 이미지 폴더 직접 스캔
                if (frames.Count == 0)
                {
                    LoadFramesFromImages(folderPath);
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

                    AddLog($"데이터 로드 완료: {frames.Count}개 프레임");
                    lstFrames.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("지원되는 데이터를 찾을 수 없습니다.\n\n지원 형식:\n- .catalog 파일\n- record_*.json 파일\n- 이미지 파일");
                }
            }
            catch (Exception ex)
            {
                AddLog($"데이터 로드 오류: {ex.Message}");
                MessageBox.Show($"데이터 로드 중 오류 발생:\n{ex.Message}");
            }
        }

        private void LoadFramesFromCatalog(string catalogFile, string basePath)
        {
            try
            {
                string catalogDir = Path.GetDirectoryName(catalogFile);
                string[] lines = File.ReadAllLines(catalogFile, Encoding.UTF8);

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // catalog 파일 형식: JSON 또는 텍스트
                    FrameData fd = ParseCatalogLine(line, catalogDir, basePath);
                    if (fd != null && !string.IsNullOrEmpty(fd.ImagePath))
                    {
                        frames.Add(fd);
                    }
                }

                AddLog($"Catalog 파일 로드됨: {Path.GetFileName(catalogFile)} ({frames.Count}개)");
            }
            catch (Exception ex)
            {
                AddLog($"Catalog 파싱 오류 ({catalogFile}): {ex.Message}");
            }
        }

        private FrameData ParseCatalogLine(string line, string catalogDir, string basePath)
        {
            try
            {
                // DonkeyCar catalog 형식: JSON 라인
                // 예: {"image_filename":"xxx.jpg","angle":0.5,"throttle":0.3}

                // 1. 이미지 파일명 추출 시도
                var imgMatch = Regex.Match(line, @"""image_filename""\s*:\s*""([^""]+)""", RegexOptions.IgnoreCase);
                if (!imgMatch.Success)
                {
                    imgMatch = Regex.Match(line, @"""image""\s*:\s*""([^""]+)""", RegexOptions.IgnoreCase);
                }
                if (!imgMatch.Success)
                {
                    imgMatch = Regex.Match(line, @"""cam/image_array""\s*:\s*""([^""]+)""", RegexOptions.IgnoreCase);
                }
                if (!imgMatch.Success) return null;

                string imagePath = imgMatch.Groups[1].Value;

                // 2. angle 추출 (다양한 필드명 시도)
                var angleMatch = Regex.Match(line, @"""user/angle""\s*:\s*([-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)", RegexOptions.IgnoreCase);
                if (!angleMatch.Success)
                {
                    angleMatch = Regex.Match(line, @"""angle""\s*:\s*([-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)", RegexOptions.IgnoreCase);
                }

                // 3. throttle 추출
                var throttleMatch = Regex.Match(line, @"""user/throttle""\s*:\s*([-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)", RegexOptions.IgnoreCase);
                if (!throttleMatch.Success)
                {
                    throttleMatch = Regex.Match(line, @"""throttle""\s*:\s*([-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)", RegexOptions.IgnoreCase);
                }

                FrameData fd = new FrameData();
                fd.ImagePath = imagePath;
                fd.Angle = angleMatch.Success ? double.Parse(angleMatch.Groups[1].Value) : 0.0;
                fd.Throttle = throttleMatch.Success ? double.Parse(throttleMatch.Groups[1].Value) : 0.0;

                return fd;
            }
            catch (Exception ex)
            {
                AddLog($"라인 파싱 오류: {ex.Message}");
                return null;
            }
        }

        private void LoadFramesFromJson(string jsonFile, string basePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(jsonFile);
                
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    FrameData fd = ParseCatalogLine(line, Path.GetDirectoryName(jsonFile), basePath);
                    if (fd != null && !string.IsNullOrEmpty(fd.ImagePath))
                    {
                        frames.Add(fd);
                    }
                }

                AddLog($"JSON 파일 로드됨: {Path.GetFileName(jsonFile)} ({frames.Count}개)");
            }
            catch (Exception ex)
            {
                AddLog($"JSON 파싱 오류 ({jsonFile}): {ex.Message}");
            }
        }

        private void LoadFramesFromImages(string folderPath)
        {
            try
            {
                // image 폴더 찾기
                string imagesPath = Path.Combine(folderPath, "image");
                if (!Directory.Exists(imagesPath))
                {
                    imagesPath = Path.Combine(folderPath, "images");
                }
                if (!Directory.Exists(imagesPath))
                {
                    imagesPath = folderPath;
                }

                // 모든 이미지 파일 찾기
                var imageFiles = Directory.GetFiles(imagesPath, "*.*", SearchOption.AllDirectories)
                    .Where(f => 
                    {
                        string ext = Path.GetExtension(f).ToLower();
                        return ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp";
                    })
                    .OrderBy(f => f)
                    .ToArray();

                // 이미지를 FrameData로 변환
                foreach (var imgFile in imageFiles)
                {
                    FrameData fd = new FrameData();
                    // .NET Framework에서 상대경로 생성
                    Uri baseUri = new Uri(folderPath + Path.DirectorySeparatorChar);
                    Uri fileUri = new Uri(imgFile);
                    fd.ImagePath = baseUri.MakeRelativeUri(fileUri).ToString().Replace('/', Path.DirectorySeparatorChar);
                    fd.Angle = 0.0;
                    fd.Throttle = 0.0;
                    frames.Add(fd);
                }

                if (imageFiles.Length > 0)
                {
                    AddLog($"이미지 폴더에서 {imageFiles.Length}개 이미지 로드됨");
                }
            }
            catch (Exception ex)
            {
                AddLog($"이미지 로드 오류: {ex.Message}");
            }
        }

        private void TrackFrame_Scroll(object sender, EventArgs e)
        {
            if (trackFrame.Value >= 0 && trackFrame.Value < frames.Count)
            {
                lstFrames.SelectedIndex = trackFrame.Value;
            }
        }

        private void LstFrames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstFrames.SelectedIndex >= 0 && lstFrames.SelectedIndex < frames.Count)
            {
                int idx = lstFrames.SelectedIndex;
                trackFrame.Value = idx;
                ShowFrame(frames[idx]);
            }
        }

        private void ShowFrame(FrameData frame)
        {
            try
            {
                string imgPath = Path.Combine(currentFolder, "image", frame.ImagePath);
                if (!File.Exists(imgPath))
                {
                    imgPath = Path.Combine(currentFolder, frame.ImagePath);
                }
                if (!File.Exists(imgPath))
                {
                    imgPath = Path.Combine(currentFolder, "images", frame.ImagePath);
                }

                if (File.Exists(imgPath))
                {
                    if (picMain.Image != null)
                    {
                        picMain.Image.Dispose();
                    }
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
                AddLog("이미지 불러오기 실패: " + ex.Message);
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
