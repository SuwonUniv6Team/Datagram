using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Datagram
{
    public class EpochData
    {
        public int    Epoch   { get; set; }
        public double Loss    { get; set; }
        public double ValLoss { get; set; }
    }

    public class GraphWindow : Form
    {
        private TabControl  tabControl;
        private TabPage     tabData;
        private TabPage     tabTrain;

        // 섹터 1
        private ComboBox    cmbCatalog;
        private RadioButton rdoAngle, rdoThrottle, rdoBoth;
        private Label       lblDataInfo;
        private DataGraphPanel dataGraph;

        // 섹터 2
        private Label       lblEpochInfo;
        private Label       lblBestVal;
        private Label       lblScore;
        private TrainGraphPanel trainGraph;

        private Dictionary<string, List<FrameData>> _catalogData = new Dictionary<string, List<FrameData>>();

        public GraphWindow()
        {
            InitUI();
        }

        private void InitUI()
        {
            this.Text            = "Datagram - 그래프 분석";
            this.Size            = new Size(900, 620);
            this.MinimumSize     = new Size(700, 500);
            this.BackColor       = Color.FromArgb(25, 25, 25);
            this.ForeColor       = Color.White;
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            tabControl = new TabControl
            {
                Dock     = DockStyle.Fill,
                DrawMode = TabDrawMode.OwnerDrawFixed,
                ItemSize = new Size(160, 32),
                Font     = new Font("Segoe UI", 10f, FontStyle.Bold)
            };
            tabControl.DrawItem += TabControl_DrawItem;

            tabData  = new TabPage("📊  데이터 분석") { BackColor = Color.FromArgb(25, 25, 25) };
            tabTrain = new TabPage("🤖  AI 학습")     { BackColor = Color.FromArgb(25, 25, 25) };

            BuildSector1(tabData);
            BuildSector2(tabTrain);

            tabControl.TabPages.Add(tabData);
            tabControl.TabPages.Add(tabTrain);
            this.Controls.Add(tabControl);
        }

        private void TabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            bool selected = e.Index == tabControl.SelectedIndex;
            Color bg = selected ? Color.FromArgb(0, 150, 100) : Color.FromArgb(40, 40, 40);
            using (SolidBrush brush = new SolidBrush(bg))
                e.Graphics.FillRectangle(brush, e.Bounds);
            TextRenderer.DrawText(e.Graphics, tabControl.TabPages[e.Index].Text,
                e.Font, e.Bounds, Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        // ════════════════════════════════════════════════════════
        // 섹터 1 : 데이터 분석
        // ════════════════════════════════════════════════════════
        private void BuildSector1(TabPage page)
        {
            Panel toolbar = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 52,
                BackColor = Color.FromArgb(35, 35, 35),
                Padding   = new Padding(8, 8, 8, 8)
            };

            Label lblCatalog = MakeLabel("카탈로그:", 10, 16);

            cmbCatalog = new ComboBox
            {
                Location      = new Point(83, 12),
                Size          = new Size(200, 28),
                BackColor     = Color.FromArgb(50, 50, 50),
                ForeColor     = Color.White,
                FlatStyle     = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCatalog.SelectedIndexChanged += CmbCatalog_Changed;

            Label lblType = MakeLabel("표시:", 300, 16);
            rdoAngle    = MakeRadio("Angle",    355, 14);
            rdoThrottle = MakeRadio("Throttle", 420, 14);
            rdoBoth     = MakeRadio("Both",     500, 14);
            rdoBoth.Checked = true;

            rdoAngle.CheckedChanged    += (s, e) => RefreshDataGraph();
            rdoThrottle.CheckedChanged += (s, e) => RefreshDataGraph();
            rdoBoth.CheckedChanged     += (s, e) => RefreshDataGraph();

            lblDataInfo = MakeLabel("메인 화면에서 폴더를 선택하면 자동으로 로드됩니다.", 560, 16);
            lblDataInfo.ForeColor = Color.FromArgb(130, 160, 130);

            toolbar.Controls.AddRange(new Control[] {
                lblCatalog, cmbCatalog,
                lblType, rdoAngle, rdoThrottle, rdoBoth, lblDataInfo });

            dataGraph = new DataGraphPanel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.FromArgb(18, 18, 18)
            };

            page.Controls.Add(dataGraph);
            page.Controls.Add(toolbar);
        }

        // ── 메인 폼에서 호출: 폴더 로드 시 카탈로그 데이터 전달 ────────────
        public void LoadFromFolder(string folderPath, List<FrameData> allFrames)
        {
            if (this.IsDisposed) return;
            this.Invoke(new Action(() =>
            {
                _catalogData.Clear();
                cmbCatalog.Items.Clear();

                // 카탈로그별로 분류
                var grouped = allFrames.GroupBy(f => f.CatalogName).OrderBy(g => g.Key);
                foreach (var group in grouped)
                    _catalogData[group.Key] = group.ToList();

                // 전체 추가
                _catalogData["[ 전체 카탈로그 ]"] = allFrames;

                cmbCatalog.Items.Add("[ 전체 카탈로그 ]");
                foreach (string key in _catalogData.Keys.Where(k => k != "[ 전체 카탈로그 ]"))
                    cmbCatalog.Items.Add(key);

                cmbCatalog.SelectedIndex = 0;
                lblDataInfo.Text = $"총 {allFrames.Count}개 프레임 로드됨";
                lblDataInfo.ForeColor = Color.FromArgb(150, 200, 150);

                // 섹터 1 탭으로 전환
                tabControl.SelectedTab = tabData;
            }));
        }

        private void CmbCatalog_Changed(object sender, EventArgs e) => RefreshDataGraph();

        private void RefreshDataGraph()
        {
            if (cmbCatalog.SelectedItem == null) return;
            string key = cmbCatalog.SelectedItem.ToString();
            if (!_catalogData.ContainsKey(key)) return;

            List<FrameData> frames = _catalogData[key];
            bool showAngle    = rdoAngle.Checked    || rdoBoth.Checked;
            bool showThrottle = rdoThrottle.Checked || rdoBoth.Checked;
            dataGraph.SetData(frames, showAngle, showThrottle);
            lblDataInfo.Text = $"총 {frames.Count}개 프레임";
            lblDataInfo.ForeColor = Color.FromArgb(150, 200, 150);
        }

        // ════════════════════════════════════════════════════════
        // 섹터 2 : AI 학습
        // ════════════════════════════════════════════════════════
        private void BuildSector2(TabPage page)
        {
            Panel infoPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 52,
                BackColor = Color.FromArgb(35, 35, 35),
                Padding   = new Padding(10, 8, 10, 8)
            };

            lblEpochInfo = MakeLabel("학습 대기 중...", 10, 16);
            lblEpochInfo.Font      = new Font("Consolas", 10f, FontStyle.Bold);
            lblEpochInfo.ForeColor = Color.FromArgb(80, 200, 255);
            lblEpochInfo.AutoSize  = true;

            lblBestVal = MakeLabel("", 400, 16);
            lblBestVal.Font      = new Font("Consolas", 10f, FontStyle.Bold);
            lblBestVal.ForeColor = Color.FromArgb(100, 255, 150);
            lblBestVal.AutoSize  = true;

            lblScore = new Label
            {
                Location  = new Point(700, 8),
                Size      = new Size(180, 36),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 14f, FontStyle.Bold),
                TextAlign = System.Drawing.ContentAlignment.MiddleRight,
                Text      = ""
            };

            infoPanel.Controls.AddRange(new Control[] { lblEpochInfo, lblBestVal, lblScore });

            trainGraph = new TrainGraphPanel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.FromArgb(18, 18, 18)
            };

            // ── 하단 설명 패널 ──
            Panel descPanel = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 58,
                BackColor = Color.FromArgb(30, 30, 30),
                Padding   = new Padding(10, 6, 10, 6)
            };

            // 설명 패널을 두 줄로 분리
            Panel descInner = new Panel { Dock = DockStyle.Fill };

            Label lblDesc = new Label
            {
                Dock      = DockStyle.Top,
                Height    = 28,
                ForeColor = Color.FromArgb(160, 160, 160),
                Font      = new Font("Segoe UI", 8.2f),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Padding   = new Padding(4, 0, 0, 0),
                Text      = "📌  학습횟수: 전체 데이터를 반복 학습한 횟수  │  학습오차: 학습 데이터의 오차 (낮을수록 좋음)  │  검증오차: 실제 성능 지표 (낮을수록 좋음)  │  최저점(★): 가장 좋은 성능의 학습 시점"
            };

            Label lblScoreDesc = new Label
            {
                Dock      = DockStyle.Top,
                Height    = 28,
                ForeColor = Color.FromArgb(130, 180, 255),
                Font      = new Font("Segoe UI", 8.2f),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Padding   = new Padding(4, 0, 0, 0),
                Text      = "🏆  점수 기준 (검증오차 기반)  │  S(90~100점): 0.01 이하  │  A(70~89점): 0.05 이하  │  B(50~69점): 0.15 이하  │  C(30~49점): 0.30 이하  │  D(0~29점): 0.30 초과"
            };

            descInner.Controls.Add(lblDesc);
            descInner.Controls.Add(lblScoreDesc);

            descPanel.Controls.Add(descInner);

            page.Controls.Add(trainGraph);
            page.Controls.Add(descPanel);
            page.Controls.Add(infoPanel);
        }

        public void AddTrainEpoch(int epoch, int totalEpochs, double loss, double valLoss)
        {
            if (this.IsDisposed) return;
            this.Invoke(new Action(() =>
            {
                trainGraph.AddEpoch(epoch, loss, valLoss);
                lblEpochInfo.Text = $"학습 {epoch} / {totalEpochs}회   |   학습오차: {loss:F6}   |   검증오차: {valLoss:F6}";
                double best = trainGraph.BestValLoss;
                lblBestVal.Text = "";
                if (epoch == 1) tabControl.SelectedTab = tabTrain;

                // 점수 계산 (100점 만점)
                UpdateScore(best, valLoss);
            }));
        }

        public void TrainFinished()
        {
            if (this.IsDisposed) return;
            this.Invoke(new Action(() =>
            {
                lblEpochInfo.Text      = $"✅ 학습 완료!   최저 검증오차: {trainGraph.BestValLoss:F6}";
                lblEpochInfo.ForeColor = Color.FromArgb(100, 255, 150);
                UpdateScore(trainGraph.BestValLoss, trainGraph.BestValLoss);
            }));
        }

        // ── 헬퍼 ─────────────────────────────────────────────────────────────
        // ── 점수 환산 (100점 만점) ──────────────────────────────────────────
        // val_loss 기준: 0.001 이하 → 100점, 0.2 이상 → 0점 (로그 스케일)
        // 점수 기준표 (val_loss 기반):
        // 0.01 이하  → 90~100점 (S)
        // 0.01~0.05  → 70~89점  (A/B)
        // 0.05~0.15  → 40~69점  (C)
        // 0.15 이상  → 0~39점   (D)
        private void UpdateScore(double bestValLoss, double currentValLoss)
        {
            if (bestValLoss >= double.MaxValue || lblScore == null) return;

            double v = bestValLoss;
            double score;

            if      (v <= 0.005) score = 100;
            else if (v <= 0.01)  score = 90 + (0.01 - v)  / (0.01 - 0.005) * 10;
            else if (v <= 0.05)  score = 70 + (0.05 - v)  / (0.05 - 0.01)  * 20;
            else if (v <= 0.15)  score = 40 + (0.15 - v)  / (0.15 - 0.05)  * 30;
            else if (v <= 0.3)   score = 10 + (0.3  - v)  / (0.3  - 0.15)  * 30;
            else                 score = Math.Max(0, 10 - (v - 0.3) * 30);

            score = Math.Max(0, Math.Min(100, score));

            Color scoreColor;
            string grade;
            if      (score >= 90) { scoreColor = Color.FromArgb(100, 255, 150); grade = "S"; }
            else if (score >= 70) { scoreColor = Color.FromArgb(80,  200, 255); grade = "A"; }
            else if (score >= 50) { scoreColor = Color.FromArgb(255, 220,  80); grade = "B"; }
            else if (score >= 30) { scoreColor = Color.FromArgb(255, 160,  60); grade = "C"; }
            else                  { scoreColor = Color.FromArgb(255,  80,  80); grade = "D"; }

            lblScore.Text      = $"{grade}  {score:F1}점";
            lblScore.ForeColor = scoreColor;
        }

        private Label MakeLabel(string text, int x, int y)
        {
            return new Label
            {
                Text      = text,
                Location  = new Point(x, y),
                ForeColor = Color.FromArgb(180, 180, 180),
                Font      = new Font("Segoe UI", 9f),
                AutoSize  = true
            };
        }

        private RadioButton MakeRadio(string text, int x, int y)
        {
            return new RadioButton
            {
                Text      = text,
                Location  = new Point(x, y),
                AutoSize  = true,
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 9f)
            };
        }
    }

    // ════════════════════════════════════════════════════════════════
    // 섹터 1 그래프 패널
    // ════════════════════════════════════════════════════════════════
    public class DataGraphPanel : Control
    {
        private List<FrameData> _frames = new List<FrameData>();
        private bool _showAngle    = true;
        private bool _showThrottle = true;
        private int  _hoveredIdx   = -1;

        private const int PL = 85, PR = 15, PT = 20, PB = 35;

        private readonly Color CAngle    = Color.FromArgb(80,  180, 255);
        private readonly Color CThrottle = Color.FromArgb(255, 140,  60);
        private readonly Color CGrid     = Color.FromArgb(42,  42,  42);
        private readonly Color CAxis     = Color.FromArgb(110, 110, 110);

        public DataGraphPanel()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
            MouseMove  += (s, e) => { UpdateHover(e.X); };
            MouseLeave += (s, e) => { _hoveredIdx = -1; Invalidate(); };
        }

        public void SetData(List<FrameData> frames, bool angle, bool throttle)
        {
            _frames       = frames;
            _showAngle    = angle;
            _showThrottle = throttle;
            _hoveredIdx   = -1;
            Invalidate();
        }

        private void UpdateHover(int mouseX)
        {
            if (_frames.Count == 0) return;
            float graphW = Width - PL - PR;
            int   idx    = (int)((mouseX - PL) / graphW * (_frames.Count - 1));
            idx = Math.Max(0, Math.Min(_frames.Count - 1, idx));
            if (idx != _hoveredIdx) { _hoveredIdx = idx; Invalidate(); }
        }

        private float FrameX(int i)
        {
            float graphW = Width - PL - PR;
            return PL + (_frames.Count <= 1 ? 0 : (float)i / (_frames.Count - 1) * graphW);
        }

        private float ValueY(double v)
        {
            float graphH = Height - PT - PB;
            return PT + graphH - (float)((v + 1.0) / 2.0) * graphH;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.FromArgb(18, 18, 18));

            if (_frames.Count == 0)
            {
                using (Font f = new Font("Segoe UI", 10f))
                {
                    string msg = "메인 화면에서 폴더를 선택하면 자동으로 표시됩니다.";
                    SizeF sz = g.MeasureString(msg, f);
                    g.DrawString(msg, f, new SolidBrush(CAxis), (Width - sz.Width) / 2, (Height - sz.Height) / 2);
                }
                return;
            }

            DrawDataGrid(g);
            DrawDataAxes(g);
            if (_showAngle)    DrawDataLine(g, true);
            if (_showThrottle) DrawDataLine(g, false);
            DrawDataLegend(g);
            if (_hoveredIdx >= 0) DrawDataTooltip(g, _hoveredIdx);
        }

        private void DrawDataGrid(Graphics g)
        {
            using (Pen pen = new Pen(CGrid, 1f) { DashStyle = DashStyle.Dash })
            using (Font font = new Font("Consolas", 7.5f))
            using (SolidBrush br = new SolidBrush(CAxis))
            {
                double[] ticks = { -1, -0.5, 0, 0.5, 1 };
                foreach (double v in ticks)
                {
                    float y = ValueY(v);
                    g.DrawLine(pen, PL, y, Width - PR, y);
                    SizeF ts2 = g.MeasureString(v.ToString("F1"), font);
                    g.DrawString(v.ToString("F1"), font, br, PL - ts2.Width - 3, y - ts2.Height / 2);
                }
            }
        }

        private void DrawDataAxes(Graphics g)
        {
            using (Pen pen = new Pen(CAxis, 1.5f))
            using (Font font = new Font("Consolas", 7.5f))
            using (SolidBrush br = new SolidBrush(CAxis))
            {
                g.DrawLine(pen, PL, PT, PL, Height - PB);
                g.DrawLine(pen, PL, Height - PB, Width - PR, Height - PB);

                int step = Math.Max(1, _frames.Count / 10);
                for (int i = 0; i < _frames.Count; i += step)
                {
                    float x = FrameX(i);
                    g.DrawString(_frames[i].FrameIndex.ToString(), font, br, x - 8, Height - PB + 4);
                }

                using (Font lf = new Font("Segoe UI", 8f))
                {
                    g.DrawString("Value", lf, br, 1, PT);
                    g.DrawString("Frame", lf, br, PL + (Width - PL - PR) / 2 - 18, Height - PB + 18);
                }
            }
        }

        private void DrawDataLine(Graphics g, bool isAngle)
        {
            if (_frames.Count < 2) return;
            Color color = isAngle ? CAngle : CThrottle;
            using (Pen pen = new Pen(color, 1.5f))
            {
                for (int i = 1; i < _frames.Count; i++)
                {
                    float x1 = FrameX(i - 1), y1 = ValueY(isAngle ? _frames[i-1].Angle : _frames[i-1].Throttle);
                    float x2 = FrameX(i),     y2 = ValueY(isAngle ? _frames[i].Angle   : _frames[i].Throttle);
                    g.DrawLine(pen, x1, y1, x2, y2);
                }
            }
        }

        private void DrawDataLegend(Graphics g)
        {
            using (Font font = new Font("Segoe UI", 8.5f))
            {
                int x = PL + 10, y = PT + 5;
                if (_showAngle)
                {
                    using (Pen pen = new Pen(CAngle, 2f)) g.DrawLine(pen, x, y + 5, x + 18, y + 5);
                    g.DrawString("Angle", font, new SolidBrush(CAngle), x + 22, y - 1);
                }
                if (_showThrottle)
                {
                    int ox = _showAngle ? 90 : 0;
                    using (Pen pen = new Pen(CThrottle, 2f)) g.DrawLine(pen, x + ox, y + 5, x + ox + 18, y + 5);
                    g.DrawString("Throttle", font, new SolidBrush(CThrottle), x + ox + 22, y - 1);
                }
            }
        }

        private void DrawDataTooltip(Graphics g, int idx)
        {
            if (idx >= _frames.Count) return;
            FrameData fd = _frames[idx];
            float x = FrameX(idx);

            using (Pen pen = new Pen(Color.FromArgb(70, 255, 255, 255), 1f) { DashStyle = DashStyle.Dash })
                g.DrawLine(pen, x, PT, x, Height - PB);

            List<string> lines = new List<string> { $" Frame    : {fd.FrameIndex}" };
            if (_showAngle)    lines.Add($" Angle    : {fd.Angle:F4}");
            if (_showThrottle) lines.Add($" Throttle : {fd.Throttle:F4}");
            lines.Add($" Catalog  : {fd.CatalogName}");

            using (Font font = new Font("Consolas", 8.5f))
            {
                float lh = font.GetHeight(g) + 3;
                float tw = 185, th = lh * lines.Count + 14;
                float tx = x + 10, ty = PT + 10;
                if (tx + tw > Width)       tx = x - tw - 10;
                if (ty + th > Height - PB) ty = Height - PB - th;

                using (SolidBrush bg = new SolidBrush(Color.FromArgb(215, 20, 20, 20)))
                using (Pen brd = new Pen(CAngle, 1.5f))
                {
                    g.FillRectangle(bg,  tx, ty, tw, th);
                    g.DrawRectangle(brd, tx, ty, tw, th);
                }

                Color[] colors = { Color.FromArgb(200,200,200), CAngle, CThrottle, Color.FromArgb(160,160,160) };
                for (int i = 0; i < lines.Count; i++)
                    g.DrawString(lines[i], font, new SolidBrush(i < colors.Length ? colors[i] : colors[0]), tx + 4, ty + 7 + i * lh);
            }
        }
    }

    // ════════════════════════════════════════════════════════════════
    // 섹터 2 그래프 패널
    // ════════════════════════════════════════════════════════════════
    public class TrainGraphPanel : Control
    {
        private List<EpochData> _data = new List<EpochData>();
        private int _hoveredIdx = -1;

        public double BestValLoss { get; private set; } = double.MaxValue;

        private const int PL = 85, PR = 15, PT = 20, PB = 35;

        private readonly Color CTrain = Color.FromArgb(80,  160, 255);
        private readonly Color CVal   = Color.FromArgb(255, 100, 100);
        private readonly Color CBest  = Color.FromArgb(100, 255, 150);
        private readonly Color CGrid  = Color.FromArgb(42,  42,  42);
        private readonly Color CAxis  = Color.FromArgb(110, 110, 110);

        public TrainGraphPanel()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
            MouseMove  += (s, e) => { UpdateHover(e.X, e.Y); };
            MouseLeave += (s, e) => { _hoveredIdx = -1; Invalidate(); };
        }

        public void AddEpoch(int epoch, double loss, double valLoss)
        {
            _data.Add(new EpochData { Epoch = epoch, Loss = loss, ValLoss = valLoss });
            if (valLoss < BestValLoss) BestValLoss = valLoss;
            Invalidate();
        }

        public void Clear()
        {
            _data.Clear();
            _hoveredIdx = -1;
            BestValLoss = double.MaxValue;
            Invalidate();
        }

        private void UpdateHover(int mx, int my)
        {
            if (_data.Count == 0) return;
            int nearest = -1; float minD = 24f;
            for (int i = 0; i < _data.Count; i++)
            {
                PointF[] pts = { ToScreen(_data[i].Epoch, _data[i].Loss), ToScreen(_data[i].Epoch, _data[i].ValLoss) };
                foreach (PointF pt in pts)
                {
                    float d = (float)Math.Sqrt(Math.Pow(mx - pt.X, 2) + Math.Pow(my - pt.Y, 2));
                    if (d < minD) { minD = d; nearest = i; }
                }
            }
            if (nearest != _hoveredIdx) { _hoveredIdx = nearest; Invalidate(); }
        }

        private PointF ToScreen(int epoch, double loss)
        {
            if (_data.Count == 0) return PointF.Empty;
            int    total  = _data[_data.Count - 1].Epoch;
            double maxL   = _data.Max(d => Math.Max(d.Loss, d.ValLoss)) * 1.1;
            float  graphW = Width  - PL - PR;
            float  graphH = Height - PT - PB;
            float  x = PL + (total <= 1 ? 0 : (float)(epoch - 1) / (total - 1) * graphW);
            float  y = PT + graphH - (float)(loss / maxL) * graphH;
            return new PointF(x, y);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.FromArgb(18, 18, 18));

            if (_data.Count == 0)
            {
                using (Font f = new Font("Segoe UI", 10f))
                {
                    string msg = "AI 학습 버튼을 누르면 실시간으로 표시됩니다.";
                    SizeF sz = g.MeasureString(msg, f);
                    g.DrawString(msg, f, new SolidBrush(CAxis), (Width - sz.Width) / 2, (Height - sz.Height) / 2);
                }
                return;
            }

            DrawTrainGrid(g);
            DrawTrainAxes(g);
            DrawBestLine(g);
            DrawTrainLines(g);
            DrawTrainPoints(g);
            DrawTrainLegend(g);
            if (_hoveredIdx >= 0) DrawTrainTooltip(g, _hoveredIdx);
        }

        private void DrawTrainGrid(Graphics g)
        {
            double maxL   = _data.Max(d => Math.Max(d.Loss, d.ValLoss)) * 1.1;
            float  graphH = Height - PT - PB;
            using (Pen pen = new Pen(CGrid, 1f) { DashStyle = DashStyle.Dash })
            using (Font font = new Font("Consolas", 7.5f))
            using (SolidBrush br = new SolidBrush(CAxis))
            {
                for (int i = 0; i <= 5; i++)
                {
                    float  y = PT + graphH * i / 5;
                    double v = maxL * (1.0 - (double)i / 5);
                    g.DrawLine(pen, PL, y, Width - PR, y);
                    SizeF ts = g.MeasureString(v.ToString("F5"), font);
                    g.DrawString(v.ToString("F5"), font, br, PL - ts.Width - 3, y - ts.Height / 2);
                }
            }
        }

        private void DrawTrainAxes(Graphics g)
        {
            using (Pen pen = new Pen(CAxis, 1.5f))
            using (Font font = new Font("Consolas", 7.5f))
            using (SolidBrush br = new SolidBrush(CAxis))
            {
                g.DrawLine(pen, PL, PT, PL, Height - PB);
                g.DrawLine(pen, PL, Height - PB, Width - PR, Height - PB);

                int step = Math.Max(1, _data.Count / 10);
                for (int i = 0; i < _data.Count; i += step)
                {
                    float x = ToScreen(_data[i].Epoch, 0).X;
                    g.DrawString(_data[i].Epoch.ToString(), font, br, x - 6, Height - PB + 4);
                }

                using (Font lf = new Font("Segoe UI", 8f))
                {
                    g.DrawString("오차",  lf, br, 2, PT);
                    g.DrawString("학습횟수", lf, br, PL + (Width - PL - PR) / 2 - 18, Height - PB + 18);
                }
            }
        }

        private void DrawBestLine(Graphics g)
        {
            if (BestValLoss >= double.MaxValue || _data.Count == 0) return;
            double maxL   = _data.Max(d => Math.Max(d.Loss, d.ValLoss)) * 1.1;
            float  graphH = Height - PT - PB;
            float  y = PT + graphH - (float)(BestValLoss / maxL) * graphH;
            using (Pen pen = new Pen(Color.FromArgb(80, CBest), 1.5f) { DashStyle = DashStyle.Dash })
            using (Font font = new Font("Consolas", 7.5f))
            using (SolidBrush br = new SolidBrush(CBest))
            {
                g.DrawLine(pen, PL, y, Width - PR, y);
                // 텍스트를 그래프 안쪽 왼쪽에 표시 (잘림 방지)
                SizeF textSize = g.MeasureString($"최저: {BestValLoss:F5}", font);
                float textX = PL + 8;
                float textY = y - textSize.Height - 2;
                if (textY < PT) textY = y + 4;
                using (SolidBrush bgBr = new SolidBrush(Color.FromArgb(180, 18, 18, 18)))
                    g.FillRectangle(bgBr, textX - 2, textY - 1, textSize.Width + 4, textSize.Height + 2);
                g.DrawString($"최저: {BestValLoss:F5}", font, br, textX, textY);
            }
        }

        private void DrawTrainLines(Graphics g)
        {
            if (_data.Count < 2) return;
            using (Pen pt = new Pen(CTrain, 2f))
            using (Pen pv = new Pen(CVal,   2f))
            {
                for (int i = 1; i < _data.Count; i++)
                {
                    g.DrawLine(pt, ToScreen(_data[i-1].Epoch, _data[i-1].Loss),    ToScreen(_data[i].Epoch, _data[i].Loss));
                    g.DrawLine(pv, ToScreen(_data[i-1].Epoch, _data[i-1].ValLoss), ToScreen(_data[i].Epoch, _data[i].ValLoss));
                }
            }
        }

        private void DrawTrainPoints(Graphics g)
        {
            foreach (EpochData d in _data)
            {
                bool   isBest = Math.Abs(d.ValLoss - BestValLoss) < 1e-9;
                PointF p      = ToScreen(d.Epoch, d.Loss);
                PointF pv     = ToScreen(d.Epoch, d.ValLoss);
                using (SolidBrush b = new SolidBrush(CTrain)) g.FillEllipse(b, p.X-3, p.Y-3, 6, 6);
                int sz = isBest ? 10 : 6, off = isBest ? 5 : 3;
                using (SolidBrush b = new SolidBrush(isBest ? CBest : CVal))
                    g.FillEllipse(b, pv.X - off, pv.Y - off, sz, sz);
            }
        }

        private void DrawTrainLegend(Graphics g)
        {
            using (Font font = new Font("Segoe UI", 8.5f))
            {
                int x = PL + 10, y = PT + 5;
                using (Pen p = new Pen(CTrain, 2f)) g.DrawLine(p, x, y+5, x+18, y+5);
                g.DrawString("학습 오차", font, new SolidBrush(CTrain), x+22,  y-1);
                using (Pen p = new Pen(CVal, 2f))   g.DrawLine(p, x+110, y+5, x+128, y+5);
                g.DrawString("검증 오차",   font, new SolidBrush(CVal),   x+132, y-1);
                using (Pen p = new Pen(CBest, 1.5f) { DashStyle = DashStyle.Dash }) g.DrawLine(p, x+210, y+5, x+228, y+5);
                g.DrawString("최저점",       font, new SolidBrush(CBest),  x+232, y-1);
            }
        }

        private void DrawTrainTooltip(Graphics g, int idx)
        {
            EpochData d     = _data[idx];
            PointF    p     = ToScreen(d.Epoch, d.Loss);
            PointF    pv    = ToScreen(d.Epoch, d.ValLoss);
            bool      isBest = Math.Abs(d.ValLoss - BestValLoss) < 1e-9;

            using (SolidBrush b = new SolidBrush(Color.White))
            {
                g.FillEllipse(b, p.X-5,  p.Y-5,  10, 10);
                g.FillEllipse(b, pv.X-5, pv.Y-5, 10, 10);
            }
            using (Pen pen = new Pen(Color.FromArgb(70, 255, 255, 255), 1f) { DashStyle = DashStyle.Dash })
                g.DrawLine(pen, p.X, PT, p.X, Height - PB);

            List<string> lines = new List<string> {
                $" 학습 횟수 : {d.Epoch}",
                $" 학습 오차 : {d.Loss:F6}",
                $" 검증 오차 : {d.ValLoss:F6}",
                $" 오차 차이 : {Math.Abs(d.Loss - d.ValLoss):F6}"
            };
            if (isBest) lines.Add(" ★ 최고 성능 학습!");

            using (Font font = new Font("Consolas", 8.5f))
            {
                float lh = font.GetHeight(g) + 3;
                float tw = 195, th = lh * lines.Count + 14;
                float tx = p.X + 10, ty = PT + 10;
                if (tx + tw > Width)       tx = p.X - tw - 10;
                if (ty + th > Height - PB) ty = Height - PB - th;

                using (SolidBrush bg = new SolidBrush(Color.FromArgb(215, 20, 20, 20)))
                using (Pen brd = new Pen(isBest ? CBest : CTrain, 1.5f))
                {
                    g.FillRectangle(bg,  tx, ty, tw, th);
                    g.DrawRectangle(brd, tx, ty, tw, th);
                }

                Color[] colors = { Color.FromArgb(200,200,200), CTrain, CVal, Color.FromArgb(160,160,160), CBest };
                for (int i = 0; i < lines.Count; i++)
                    g.DrawString(lines[i], font, new SolidBrush(i < colors.Length ? colors[i] : colors[0]), tx + 4, ty + 7 + i * lh);
            }
        }
    }
}
