import argparse
import os
import cv2
import numpy as np

from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import Conv2D, MaxPooling2D, Flatten, Dense
from tensorflow.keras.utils import to_categorical

parser = argparse.ArgumentParser()
parser.add_argument("--image_folder", required=True)
args = parser.parse_args()

image_folder = args.image_folder

print(f"이미지 폴더: {image_folder}")

images = []
labels = []

class_names = ["left", "center", "right"]

for label_index, class_name in enumerate(class_names):

    folder_path = os.path.join(image_folder, class_name)

    if not os.path.exists(folder_path):
        print(f"폴더 없음: {folder_path}")
        continue

    for file_name in os.listdir(folder_path):

        if file_name.lower().endswith((".jpg", ".png", ".jpeg")):

            file_path = os.path.join(folder_path, file_name)

            image = cv2.imread(file_path)

            if image is None:
                continue

            image = cv2.resize(image, (160, 120))
            image = image / 255.0

            images.append(image)
            labels.append(label_index)

print(f"로드 이미지 수: {len(images)}")

if len(images) == 0:
    raise Exception("이미지가 없습니다.")

X = np.array(images)
y = to_categorical(labels, num_classes=len(class_names))

model = Sequential([
    Conv2D(24, (5,5), activation='relu', input_shape=(120,160,3)),
    MaxPooling2D(),

    Conv2D(36, (5,5), activation='relu'),
    MaxPooling2D(),

    Flatten(),

    Dense(100, activation='relu'),
    Dense(50, activation='relu'),

    Dense(len(class_names), activation='softmax')
])

model.compile(
    optimizer='adam',
    loss='categorical_crossentropy',
    metrics=['accuracy']
)

print("AI 학습 시작")

model.fit(
    X,
    y,
    epochs=10,
    batch_size=16
)

model.save("trained_model.h5")

print("학습 완료")
print("trained_model.h5 저장 완료")