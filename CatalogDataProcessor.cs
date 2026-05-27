using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Datagram
{
    public class CatalogDataProcessor
    {
        public List<FrameData> LoadFrames(string folderPath)
        {
            var frames = new List<FrameData>();

            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
            {
                return frames;
            }

            var catalogFiles = Directory.GetFiles(folderPath, "*.catalog", SearchOption.AllDirectories)
                .Union(Directory.GetFiles(folderPath, "*.catalog_manifest", SearchOption.AllDirectories))
                .Distinct()
                .ToArray();

            foreach (var catalogFile in catalogFiles)
            {
                LoadFromTextFile(catalogFile, folderPath, frames);
            }

            if (frames.Count == 0)
            {
                var recordFiles = Directory.GetFiles(folderPath, "record_*.json", SearchOption.AllDirectories)
                    .Distinct()
                    .ToArray();

                foreach (var recordFile in recordFiles)
                {
                    LoadFromTextFile(recordFile, folderPath, frames);
                }
            }

            if (frames.Count == 0)
            {
                LoadFromImages(folderPath, frames);
            }

            ValidateAndCleanFrames(frames, folderPath, true);
            NormalizeFrames(frames);
            return frames;
        }

        public List<string> ValidateAndCleanFrames(List<FrameData> frames, string rootFolder, bool removeMissingImages)
        {
            var issues = new List<string>();
            if (frames == null)
            {
                return issues;
            }

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int i = frames.Count - 1; i >= 0; i--)
            {
                FrameData frame = frames[i];
                if (frame == null)
                {
                    frames.RemoveAt(i);
                    issues.Add("null frame 제거");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(frame.ImagePath))
                {
                    frames.RemoveAt(i);
                    issues.Add("이미지 경로가 비어 있는 프레임 제거");
                    continue;
                }

                if (double.IsNaN(frame.Angle) || double.IsInfinity(frame.Angle))
                {
                    frame.Angle = 0.0;
                    issues.Add($"angle 보정: {frame.ImagePath}");
                }

                if (double.IsNaN(frame.Throttle) || double.IsInfinity(frame.Throttle))
                {
                    frame.Throttle = 0.0;
                    issues.Add($"throttle 보정: {frame.ImagePath}");
                }

                string resolvedImagePath = ResolveImagePath(rootFolder, frame.ImagePath);
                if (string.IsNullOrWhiteSpace(resolvedImagePath) || !File.Exists(resolvedImagePath))
                {
                    if (removeMissingImages)
                    {
                        frames.RemoveAt(i);
                        issues.Add($"이미지 없음으로 제거: {frame.ImagePath}");
                        continue;
                    }
                }

                string dedupKey = (frame.ImagePath ?? string.Empty).Trim().ToLowerInvariant() + "|" + frame.Angle.ToString(CultureInfo.InvariantCulture) + "|" + frame.Throttle.ToString(CultureInfo.InvariantCulture);
                if (!seen.Add(dedupKey))
                {
                    frames.RemoveAt(i);
                    issues.Add($"중복 프레임 제거: {frame.ImagePath}");
                }
            }

            return issues;
        }

        public void NormalizeFrames(IList<FrameData> frames)
        {
            if (frames == null)
            {
                return;
            }

            for (int i = 0; i < frames.Count; i++)
            {
                frames[i].FrameIndex = i;
            }
        }

        public int DeleteFrames(List<FrameData> frames, IEnumerable<int> selectedIndices, string rootFolder, bool deleteImages, bool saveCatalogs)
        {
            if (frames == null || selectedIndices == null)
            {
                return 0;
            }

            var ordered = selectedIndices.Distinct().OrderByDescending(i => i).ToList();
            int removedCount = 0;

            foreach (int idx in ordered)
            {
                if (idx < 0 || idx >= frames.Count)
                {
                    continue;
                }

                FrameData frame = frames[idx];
                if (deleteImages)
                {
                    DeleteImageFile(rootFolder, frame);
                }

                frames.RemoveAt(idx);
                removedCount++;
            }

            NormalizeFrames(frames);

            if (saveCatalogs)
            {
                SaveCatalogSources(frames);
                ExportTrainingManifest(rootFolder, frames);
            }

            return removedCount;
        }

        public void SaveCatalogSources(IEnumerable<FrameData> frames)
        {
            if (frames == null)
            {
                return;
            }

            var grouped = frames
                .Where(f => !string.IsNullOrWhiteSpace(f.SourceFile))
                .GroupBy(f => f.SourceFile, StringComparer.OrdinalIgnoreCase);

            foreach (var group in grouped)
            {
                var lines = new List<string>();
                foreach (var frame in group)
                {
                    if (!string.IsNullOrWhiteSpace(frame.SourceLine))
                    {
                        lines.Add(frame.SourceLine);
                    }
                    else
                    {
                        lines.Add(ToCatalogJsonLine(frame));
                    }
                }

                File.WriteAllLines(group.Key, lines.ToArray(), Encoding.UTF8);
            }
        }

        public string ExportTrainingManifest(string rootFolder, IEnumerable<FrameData> frames)
        {
            if (string.IsNullOrWhiteSpace(rootFolder) || frames == null)
            {
                return null;
            }

            string manifestPath = Path.Combine(rootFolder, "training_manifest.csv");
            var lines = new List<string> { "image_filename,angle,throttle" };

            foreach (var frame in frames)
            {
                if (frame == null || string.IsNullOrWhiteSpace(frame.ImagePath))
                {
                    continue;
                }

                string imagePath = frame.ImagePath.Replace("\"", "\"\"");
                string angle = frame.Angle.ToString(CultureInfo.InvariantCulture);
                string throttle = frame.Throttle.ToString(CultureInfo.InvariantCulture);
                lines.Add($"\"{imagePath}\",{angle},{throttle}");
            }

            File.WriteAllLines(manifestPath, lines.ToArray(), Encoding.UTF8);
            return manifestPath;
        }

        public string ResolveImagePath(string rootFolder, string imagePath)
        {
            if (string.IsNullOrWhiteSpace(rootFolder) || string.IsNullOrWhiteSpace(imagePath))
            {
                return null;
            }

            if (Path.IsPathRooted(imagePath) && File.Exists(imagePath))
            {
                return imagePath;
            }

            string normalized = imagePath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            string candidate = Path.Combine(rootFolder, normalized);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            candidate = Path.Combine(rootFolder, "image", Path.GetFileName(normalized));
            if (File.Exists(candidate))
            {
                return candidate;
            }

            candidate = Path.Combine(rootFolder, "images", Path.GetFileName(normalized));
            if (File.Exists(candidate))
            {
                return candidate;
            }

            candidate = Path.Combine(rootFolder, Path.GetFileName(normalized));
            if (File.Exists(candidate))
            {
                return candidate;
            }

            return null;
        }

        private void LoadFromTextFile(string filePath, string rootFolder, List<FrameData> frames)
        {
            string[] lines;

            try
            {
                lines = File.ReadAllLines(filePath, Encoding.UTF8);
            }
            catch
            {
                lines = File.ReadAllLines(filePath);
            }

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                FrameData frame = ParseLine(line, filePath, rootFolder);
                if (frame != null)
                {
                    frames.Add(frame);
                }
            }
        }

        private FrameData ParseLine(string line, string sourceFile, string rootFolder)
        {
            var imageMatch = Regex.Match(line, @"""(image_filename|image|cam/image_array)""\s*:\s*""([^""]+)""", RegexOptions.IgnoreCase);
            if (!imageMatch.Success)
            {
                return null;
            }

            var angleMatch = Regex.Match(line, @"""(user/angle|angle)""\s*:\s*([-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)", RegexOptions.IgnoreCase);
            var throttleMatch = Regex.Match(line, @"""(user/throttle|throttle)""\s*:\s*([-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)", RegexOptions.IgnoreCase);

            string imagePath = imageMatch.Groups[2].Value.Trim();
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return null;
            }

            return new FrameData
            {
                ImagePath = imagePath,
                Angle = angleMatch.Success ? ParseDouble(angleMatch.Groups[2].Value) : 0.0,
                Throttle = throttleMatch.Success ? ParseDouble(throttleMatch.Groups[2].Value) : 0.0,
                SourceFile = sourceFile,
                SourceLine = line
            };
        }

        private void LoadFromImages(string rootFolder, List<FrameData> frames)
        {
            string imageFolder = Path.Combine(rootFolder, "image");
            if (!Directory.Exists(imageFolder))
            {
                imageFolder = Path.Combine(rootFolder, "images");
            }
            if (!Directory.Exists(imageFolder))
            {
                imageFolder = rootFolder;
            }

            var imageFiles = Directory.GetFiles(imageFolder, "*.*", SearchOption.AllDirectories)
                .Where(f =>
                {
                    string ext = Path.GetExtension(f).ToLowerInvariant();
                    return ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp";
                })
                .OrderBy(f => f)
                .ToArray();

            foreach (var imgFile in imageFiles)
            {
                frames.Add(new FrameData
                {
                    ImagePath = ToRelativePath(rootFolder, imgFile),
                    Angle = 0.0,
                    Throttle = 0.0,
                    SourceFile = null,
                    SourceLine = null
                });
            }
        }

        private static string ToRelativePath(string rootFolder, string filePath)
        {
            Uri rootUri = new Uri(AppendDirectorySeparatorChar(rootFolder));
            Uri fileUri = new Uri(filePath);
            return Uri.UnescapeDataString(rootUri.MakeRelativeUri(fileUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        private static string AppendDirectorySeparatorChar(string path)
        {
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                return path + Path.DirectorySeparatorChar;
            }

            return path;
        }

        private static double ParseDouble(string value)
        {
            double result;
            return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result)
                ? result
                : 0.0;
        }

        private static void DeleteImageFile(string rootFolder, FrameData frame)
        {
            string imagePath = null;
            if (frame != null)
            {
                imagePath = frame.ImagePath;
            }

            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return;
            }

            string resolved = Path.IsPathRooted(imagePath) ? imagePath : Path.Combine(rootFolder, imagePath);
            if (File.Exists(resolved))
            {
                File.Delete(resolved);
                return;
            }

            string alt = Path.Combine(rootFolder, "image", Path.GetFileName(imagePath));
            if (File.Exists(alt))
            {
                File.Delete(alt);
                return;
            }

            alt = Path.Combine(rootFolder, "images", Path.GetFileName(imagePath));
            if (File.Exists(alt))
            {
                File.Delete(alt);
            }
        }

        private static string ToCatalogJsonLine(FrameData frame)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{{\"image_filename\":\"{0}\",\"angle\":{1},\"throttle\":{2}}}",
                EscapeJson(frame.ImagePath),
                frame.Angle,
                frame.Throttle);
        }

        private static string EscapeJson(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
