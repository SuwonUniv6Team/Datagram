"""
Donkey Car 모델 검증 스크립트
호출: python predict.py --model "path/to/donkey_model.h5" --image_folder "path/to/data"
"""

import os
import sys
import glob
import json
import argparse
import numpy as np

os.environ['TF_CPP_MIN_LOG_LEVEL'] = '3'
os.environ['TF_ENABLE_ONEDNN_OPTS'] = '0'

def log(msg):
    print(f"[LOG] {msg}", flush=True)

def error(msg):
    print(f"[ERROR] {msg}", flush=True)
    sys.exit(1)

log("라이브러리 불러오는 중...")

try:
    import cv2
except ImportError:
    error("설치 필요: pip install opencv-python")

try:
    import tensorflow as tf
    tf.get_logger().setLevel('ERROR')
    import absl.logging
    absl.logging.set_verbosity(absl.logging.ERROR)
except ImportError:
    error("설치 필요: pip install tensorflow")

def load_image(img_path, img_size=(120, 160)):
    img = cv2.imread(img_path)
    if img is None:
        return None
    img = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
    img = cv2.resize(img, (img_size[1], img_size[0]))
    img = img.astype(np.float32) / 255.0
    return img

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--model",        required=True)
    parser.add_argument("--image_folder", required=True)
    args = parser.parse_args()

    if not os.path.isfile(args.model):
        error(f"모델 파일 없음: {args.model}")
    if not os.path.isdir(args.image_folder):
        error(f"폴더 없음: {args.image_folder}")

    log(f"모델 불러오는 중: {args.model}")
    model = tf.keras.models.load_model(args.model, compile=False)

    # catalog에서 이미지 파일명 목록 로드 (순서 보장)
    catalog_files = sorted([
        f for f in glob.glob(os.path.join(args.image_folder, "*.catalog"))
        if not f.endswith(".catalog_manifest")
    ])

    if not catalog_files:
        error(f"catalog 파일 없음: {args.image_folder}")

    records = []
    for path in catalog_files:
        with open(path, "r", encoding="utf-8-sig") as f:
            for line in f:
                line = line.strip()
                if not line:
                    continue
                try:
                    records.append(json.loads(line))
                except json.JSONDecodeError:
                    continue

    log(f"총 {len(records)}개 프레임 이미지 불러오는 중...")

    images_dir = os.path.join(args.image_folder, "images")
    img_names = []
    imgs = []

    for rec in records:
        img_name = rec.get("cam/image_array", "")
        img = load_image(os.path.join(images_dir, img_name))
        if img is None:
            continue
        img_names.append(img_name)
        imgs.append(img)

    if not imgs:
        error("유효한 이미지가 없습니다.")

    log(f"{len(imgs)}개 이미지 배치 예측 시작...")
    X = np.array(imgs, dtype=np.float32)
    outputs = model.predict(X, batch_size=64, verbose=0)

    # n_outputs0 = angle, n_outputs1 = throttle
    if isinstance(outputs, list):
        angles = outputs[0].flatten()
    else:
        angles = outputs.flatten()

    for img_name, angle in zip(img_names, angles):
        angle = float(np.clip(angle, -1.0, 1.0))
        print(f"[PRED] image={img_name} angle={angle:.6f}", flush=True)

    log(f"예측 완료: {len(img_names)}개")
    print("[PRED_DONE]", flush=True)

if __name__ == "__main__":
    main()
