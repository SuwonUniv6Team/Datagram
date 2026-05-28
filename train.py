"""
Donkey Car 자율주행 AI 학습 스크립트
출력: donkey_model.h5  (Donkey Car 시뮬레이터 호환)
호출: python train.py --image_folder "C:/경로/data"
"""

import os
import sys
import json
import glob
import argparse
import numpy as np

def log(msg):
    print(f"[LOG] {msg}", flush=True)

def progress(epoch, total, loss, val_loss):
    print(f"[PROGRESS] epoch={epoch}/{total} loss={loss:.4f} val_loss={val_loss:.4f}", flush=True)

def done(model_path):
    print(f"[DONE] {model_path}", flush=True)

def error(msg):
    print(f"[ERROR] {msg}", flush=True)
    sys.exit(1)


# ── 라이브러리 임포트 ────────────────────────────────────────────────────────
log("라이브러리 로딩 중...")

missing = []
try:
    import cv2
except ImportError:
    missing.append("opencv-python")

try:
    from sklearn.model_selection import train_test_split
except ImportError:
    missing.append("scikit-learn")

try:
    import tensorflow as tf
    from tensorflow.keras import layers, models, callbacks
    # TensorFlow 잡음 로그 억제
    os.environ['TF_CPP_MIN_LOG_LEVEL'] = '3'
    tf.get_logger().setLevel('ERROR')
except ImportError:
    missing.append("tensorflow")

if missing:
    error(f"설치 필요: pip install {' '.join(missing)}")


# ── 데이터 로더 ──────────────────────────────────────────────────────────────
def load_dataset(data_dir, img_size=(120, 160)):
    catalog_files = sorted(glob.glob(os.path.join(data_dir, "*.catalog")))
    if not catalog_files:
        error(f"catalog 파일 없음: {data_dir}")

    records = []
    for path in catalog_files:
        with open(path, "r", encoding="utf-8") as f:
            for line in f:
                line = line.strip()
                if line:
                    records.append(json.loads(line))
    log(f"레코드 {len(records)}개 로드 (catalog {len(catalog_files)}개)")

    images_dir = os.path.join(data_dir, "images")
    X, Y_angle, Y_throttle = [], [], []
    missing_count = 0

    for rec in records:
        img_path = os.path.join(images_dir, rec.get("cam/image_array", ""))
        if not os.path.exists(img_path):
            missing_count += 1
            continue

        img = cv2.imread(img_path)
        if img is None:
            missing_count += 1
            continue

        img = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
        img = cv2.resize(img, (img_size[1], img_size[0]))
        img = img.astype(np.float32) / 255.0

        X.append(img)
        Y_angle.append(float(rec["user/angle"]))
        Y_throttle.append(float(rec["user/throttle"]))

    if missing_count:
        log(f"이미지 누락 {missing_count}개 건너뜀")

    X        = np.array(X,        dtype=np.float32)
    Y_angle  = np.array(Y_angle,  dtype=np.float32)
    Y_throttle = np.array(Y_throttle, dtype=np.float32)

    log(f"데이터셋 완료: {X.shape[0]}개 이미지")
    return X, Y_angle, Y_throttle


# ── Donkey Car 호환 모델 ─────────────────────────────────────────────────────
# Donkey Car는 angle / throttle 출력을 별도 이름의 레이어로 기대함
def build_donkey_model(input_shape=(120, 160, 3)):
    img_in = layers.Input(shape=input_shape, name="img_in")

    x = layers.Conv2D(24, 5, strides=2, activation="relu")(img_in)
    x = layers.Conv2D(32, 5, strides=2, activation="relu")(x)
    x = layers.Conv2D(64, 5, strides=2, activation="relu")(x)
    x = layers.Conv2D(64, 3, activation="relu")(x)
    x = layers.Conv2D(64, 3, activation="relu")(x)
    x = layers.Flatten()(x)
    x = layers.Dense(100, activation="relu")(x)
    x = layers.Dropout(0.1)(x)
    x = layers.Dense(50, activation="relu")(x)
    x = layers.Dropout(0.1)(x)

    # Donkey Car 호환: 출력 레이어 이름을 "n_outputs0", "n_outputs1" 로 지정
    angle_out    = layers.Dense(1, activation="tanh", name="n_outputs0")(x)
    throttle_out = layers.Dense(1, activation="tanh", name="n_outputs1")(x)

    model = models.Model(inputs=img_in, outputs=[angle_out, throttle_out])
    model.compile(
        optimizer=tf.keras.optimizers.Adam(1e-4),
        loss={"n_outputs0": "mse", "n_outputs1": "mse"},
        metrics={"n_outputs0": "mae", "n_outputs1": "mae"}
    )
    return model


class ProgressCallback(callbacks.Callback):
    def __init__(self, total):
        super().__init__()
        self.total = total

    def on_epoch_end(self, epoch, logs=None):
        logs = logs or {}
        progress(epoch + 1, self.total,
                 logs.get("loss", 0), logs.get("val_loss", 0))


# ── 메인 ─────────────────────────────────────────────────────────────────────
def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--image_folder", required=True)
    parser.add_argument("--output_dir",   default=None)
    parser.add_argument("--epochs",       type=int, default=30)
    parser.add_argument("--batch_size",   type=int, default=64)
    args = parser.parse_args()

    data_dir   = args.image_folder
    output_dir = args.output_dir or os.path.join(data_dir, "models")

    if not os.path.isdir(data_dir):
        error(f"폴더 없음: {data_dir}")

    os.makedirs(output_dir, exist_ok=True)

    log("데이터 로딩 중...")
    X, Y_angle, Y_throttle = load_dataset(data_dir)
    if len(X) == 0:
        error("유효한 이미지가 없습니다.")

    # 학습/검증 분리 (8:2)
    from sklearn.model_selection import train_test_split
    idx = np.arange(len(X))
    idx_tr, idx_val = train_test_split(idx, test_size=0.2, random_state=42)

    X_tr,  X_val  = X[idx_tr],          X[idx_val]
    Ya_tr, Ya_val = Y_angle[idx_tr],     Y_angle[idx_val]
    Yt_tr, Yt_val = Y_throttle[idx_tr],  Y_throttle[idx_val]

    log(f"학습 {len(X_tr)}개 / 검증 {len(X_val)}개")

    log("모델 생성 중...")
    model = build_donkey_model(input_shape=X.shape[1:])

    # ── .h5 로 저장 (Donkey Car 호환 포맷)
    model_path = os.path.join(output_dir, "donkey_model.h5")

    cb_list = [
        ProgressCallback(args.epochs),
        callbacks.ModelCheckpoint(
            model_path, monitor="val_loss",
            save_best_only=True, verbose=0
        ),
        callbacks.EarlyStopping(
            monitor="val_loss", patience=7,
            restore_best_weights=True, verbose=0
        ),
        callbacks.ReduceLROnPlateau(
            monitor="val_loss", factor=0.5,
            patience=3, min_lr=1e-6, verbose=0
        ),
    ]

    log(f"학습 시작 (epochs={args.epochs}, batch={args.batch_size})")
    model.fit(
        X_tr,
        {"n_outputs0": Ya_tr, "n_outputs1": Yt_tr},
        validation_data=(X_val, {"n_outputs0": Ya_val, "n_outputs1": Yt_val}),
        epochs=args.epochs,
        batch_size=args.batch_size,
        callbacks=cb_list,
        verbose=0
    )

    if not os.path.exists(model_path):
        model.save(model_path)

    log(f"모델 저장 완료: {model_path}")
    done(model_path)


if __name__ == "__main__":
    main()
