using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Datagram
{
    public class TrainStarter
    {
        private readonly RichTextBox _txtLog;

        public TrainStarter(RichTextBox txtLog)
        {
            _txtLog = txtLog;
        }

        // ── 외부에서 호출하는 메인 함수 ──────────────────────────────────────
        // 설치까지 완료되면 true 반환, 실패하면 false 반환
        public bool EnsureReady()
        {
            // 1. Python 설치 확인
            if (!IsPythonInstalled())
            {
                AddLog("⚠️ Python이 설치되어 있지 않습니다.");
                var result = MessageBox.Show(
                    "Python이 설치되어 있지 않습니다.\n자동으로 설치하시겠습니까?",
                    "Python 미설치", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    if (!InstallPython())
                    {
                        MessageBox.Show("Python 설치에 실패했습니다.\nhttps://www.python.org 에서 직접 설치해주세요.",
                            "설치 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                else return false;
            }

            AddLog("✅ Python 확인 완료");

            // 2. 필수 패키지 설치 확인 및 자동 설치
            string[] packages = { "opencv-python", "scikit-learn", "tensorflow", "matplotlib" };
            string[] importNames = { "cv2", "sklearn", "tensorflow", "matplotlib" };

            for (int i = 0; i < packages.Length; i++)
            {
                if (!IsPackageInstalled(importNames[i]))
                {
                    AddLog($"📦 {packages[i]} 설치 중...");
                    if (!InstallPackage(packages[i]))
                    {
                        MessageBox.Show($"{packages[i]} 설치에 실패했습니다.\n수동으로 설치해주세요:\npip install {packages[i]}",
                            "설치 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    AddLog($"✅ {packages[i]} 설치 완료");
                }
                else
                {
                    AddLog($"✅ {packages[i]} 확인 완료");
                }
            }

            return true;
        }

        // ── Python 설치 여부 확인 ─────────────────────────────────────────────
        private bool IsPythonInstalled()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = "--version",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                var p = Process.Start(psi);
                p.WaitForExit(5000);
                return p.ExitCode == 0;
            }
            catch { return false; }
        }

        // ── 패키지 설치 여부 확인 ─────────────────────────────────────────────
        private bool IsPackageInstalled(string importName)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"-c \"import {importName}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                var p = Process.Start(psi);
                p.WaitForExit(10000);
                return p.ExitCode == 0;
            }
            catch { return false; }
        }

        // ── pip으로 패키지 설치 ───────────────────────────────────────────────
        private bool InstallPackage(string packageName)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"-m pip install {packageName}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                var p = new Process { StartInfo = psi };

                p.OutputDataReceived += (s, e) => {
                    if (!string.IsNullOrEmpty(e.Data))
                        AddLog(e.Data);
                };
                p.ErrorDataReceived += (s, e) => {
                    if (!string.IsNullOrEmpty(e.Data))
                        AddLog(e.Data);
                };

                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                p.WaitForExit(300000); // 최대 5분 대기

                return p.ExitCode == 0;
            }
            catch { return false; }
        }

        // ── Python 자동 설치 (winget 사용) ───────────────────────────────────
        private bool InstallPython()
        {
            try
            {
                AddLog("🐍 Python 설치 중... (잠시 기다려주세요)");
                var psi = new ProcessStartInfo
                {
                    FileName = "winget",
                    Arguments = "install -e --id Python.Python.3.11 --silent --accept-package-agreements --accept-source-agreements",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                var p = new Process { StartInfo = psi };
                p.OutputDataReceived += (s, e) => {
                    if (!string.IsNullOrEmpty(e.Data)) AddLog(e.Data);
                };
                p.Start();
                p.BeginOutputReadLine();
                p.WaitForExit(600000); // 최대 10분

                if (p.ExitCode == 0)
                {
                    AddLog("✅ Python 설치 완료! 프로그램을 재시작해주세요.");
                    MessageBox.Show("Python 설치가 완료되었습니다.\n프로그램을 재시작한 후 다시 시도해주세요.",
                        "설치 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return p.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        private void AddLog(string msg)
        {
            if (_txtLog.InvokeRequired)
                _txtLog.Invoke(new Action(() => AddLog(msg)));
            else
            {
                string time = DateTime.Now.ToString("HH:mm:ss");
                _txtLog.AppendText($"[{time}] {msg}\r\n");
                _txtLog.SelectionStart = _txtLog.Text.Length;
                _txtLog.ScrollToCaret();
            }
        }
    }
}
