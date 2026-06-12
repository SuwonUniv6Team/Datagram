# Autonomous Driving Dataset Manager & Trainer

## Tech Stack

![C#](https://img.shields.io/badge/C%23-239120?style=flat-square\&logo=csharp\&logoColor=white)
![WinForms](https://img.shields.io/badge/WinForms-512BD4?style=flat-square\&logo=.net\&logoColor=white)
![Python](https://img.shields.io/badge/Python-3776AB?style=flat-square\&logo=python\&logoColor=white)
![TensorFlow](https://img.shields.io/badge/TensorFlow-FF6F00?style=flat-square\&logo=tensorflow\&logoColor=white)
![Keras](https://img.shields.io/badge/Keras-D00000?style=flat-square\&logo=keras\&logoColor=white)
![JSON](https://img.shields.io/badge/JSON-000000?style=flat-square\&logo=json\&logoColor=white)
![Platform](https://img.shields.io/badge/Platform-Windows-blue)
![AI](https://img.shields.io/badge/AI-TensorFlow-orange)
![Status](https://img.shields.io/badge/Status-Completed-success)

### 개발 기간

2026.05.01 ~ 2026.06.11

---

## 프로젝트 소개

DonkeyCar 주행 데이터를 기반으로 데이터 관리, 정제, AI 학습 및 검증 기능을 제공하는 자율주행 학습 프로그램입니다.

주행 데이터 분석부터 CNN 기반 모델 학습, 실제 데이터와 예측 결과 비교까지 하나의 WinForms 환경에서 수행할 수 있도록 구현하였습니다.

또한 WinForms 기반 GUI를 통해 데이터 관리부터 AI 학습 및 검증까지의 전 과정을 직관적으로 수행할 수 있도록 설계하였습니다.

---

## 개발 및 테스트 환경

### 개발 환경

| 구분 | 내용 |
|--------|--------|
| 개발 언어 | C#, Python 3.11.9 |
| 프레임워크 | .NET Framework 4.7.2 |
| UI Framework | Windows Forms |
| IDE | Visual Studio 2022 |
| AI Framework | TensorFlow 2.21.0, Keras 3.14.1 |
| 데이터 처리 | NumPy 2.4.6, OpenCV 4.13.0 |
| 데이터 포맷 | JSON, Catalog |
| 실행 환경 | Windows 11 |
| 연동 환경 | Python, DonkeyCar 5.2.0, WSL Ubuntu 22.04 LTS |
| 형상 관리 | Git, GitHub |

### 테스트 환경

| 구분 | 내용 |
|--------|--------|
| 운영체제 | Windows 11, Ubuntu 22.04 LTS (WSL) |
| AI 모델 | CNN 기반 Multi-Output 조향각/속도 예측 모델 |
| 학습 프레임워크 | TensorFlow 2.21.0, Keras 3.14.1 |
| 데이터셋 | DonkeyCar Simulator 주행 데이터 |
| 모델 저장 형식 | HDF5 (.h5) |
| 검증 방식 | 실제 주행 데이터와 AI 예측 결과 비교 분석 |

### 주요 라이브러리

| 라이브러리 | 버전 | 용도 |
|--------|--------|--------|
| TensorFlow | 2.21.0 | CNN 모델 학습 |
| Keras | 3.14.1 | 딥러닝 모델 구성 |
| NumPy | 2.4.6 | 데이터 전처리 |
| OpenCV | 4.13.0 | 이미지 로드 및 전처리 |
| Newtonsoft.Json | 13.0.3 | JSON 데이터 처리 |

## 프로젝트 목표

* DonkeyCar Simulator 기반 주행 데이터 관리 및 활용
* 데이터 시각화 및 품질 검증
* 불필요한 주행 데이터 필터링
* 학습 데이터 관리 자동화
* C# 기반 GUI와 Python 기반 AI 학습 환경 연동
* 학습 결과 그래프 분석 기능 제공

---

## 프로그램 시연

### 데이터 필터링
<img width="950" height="491" alt="Image" src="https://github.com/user-attachments/assets/fcfb8dab-6c5a-4aae-80da-1f16fc59d052" />

### AI 학습 실행
<img width="950" height="491" alt="Image" src="https://github.com/user-attachments/assets/b452cb1a-56c6-4b35-8032-654063093127" />

### AI 예측 비교
<img width="950" height="491" alt="Image" src="https://github.com/user-attachments/assets/8e025224-728a-4f0a-97d3-1d22631f8eae" />

---

## 주요 기능

### 데이터 관리

* DonkeyCar 주행 데이터 폴더 로드
* 이미지 프레임 확인
* Angle / Throttle 값 확인
* 프레임 단위 탐색 기능

### 데이터 재생

* 프레임 자동 재생
* 재생 속도 조절 (1x, 2x, 4x, 8x)
* 이전 / 다음 프레임 이동

### 데이터 필터링

* 직진 데이터 필터
* 급커브 데이터 필터
* 주행 여부 필터
* 카탈로그별 데이터 필터
* 필터 초기화 기능

### 데이터 정제

* 프레임 단위 데이터 삭제
* 범위 및 다중 선택 삭제
* 이미지 / JSON / Catalog 데이터 동기화
* 삭제 데이터 백업 및 복원
* 학습 데이터셋 관리

### AI 학습

* Python 학습 스크립트 연동
* Epoch 설정 및 학습 진행 모니터링
* Loss / Validation Loss 실시간 출력
* 학습 모델(.h5) 저장

### 그래프 분석

#### 데이터 분석

* Angle / Throttle 변화량 그래프
* 카탈로그별 데이터 분석
* 전체 데이터 분석

#### AI 주행 비교 분석

* 학습된 모델(.h5) 불러오기
* 프레임 단위 AI 예측 수행
* 실제 조향 데이터(Actual Steering) 시각화
* AI 예측 조향 데이터(Predicted Steering) 시각화
* 실제 데이터와 예측 결과 비교
* 조향각 오버레이 시각화
* 주행 패턴 차이 분석
* 모델 성능 검증 지원

#### 학습 분석

* Loss 그래프
* Validation Loss 그래프
* Best Validation Loss 추적
* 학습 결과 확인
* Validation Loss 기반 성능 비교

---

## 담당 역할

### 데이터 관리 UI 개발

* WinForms 기반 GUI 설계
* 데이터 탐색 인터페이스 구현
* 프레임 재생 기능 개발
* 데이터 필터링 기능 구현

### 데이터 정제 시스템 개발

* 주행 데이터 필터링 및 삭제 기능 구현
* 이미지 / JSON / Catalog 데이터 동기화 처리
* 삭제 데이터 백업 및 복원 기능 구현
* 학습 데이터셋 관리 기능 개발

### AI 학습 연동

* Python 학습 스크립트 연동
* 실시간 학습 로그 출력 기능 제공
* 학습 횟수(Epoch) 설정 기능 제공
* 모델 저장 기능 구현

### 데이터 분석 도구 개발

* Angle / Throttle 그래프 시각화
* 카탈로그 기반 데이터 분포 분석 기능 구현
* 실제 조향 데이터와 AI 예측 데이터 비교 기능 구현
* 조향각 오버레이 시각화 기능 구현
* 학습 결과(Loss / Validation Loss) 시각화
* Validation Loss 기반 성능 평가 기능 구현
* AI 모델 검증 도구 구현

---

## 시스템 구조

```text
주행 데이터 생성 및 수집
↓
데이터 관리 및 정제
↓
삭제 데이터 백업
↓
정제된 데이터셋
↓
AI 학습
↓
모델(.h5) 생성
↓
Loss / Validation Loss 분석
↓
AI 검증
↓
실제 데이터 ↔ AI 예측 비교
```

## 폴더 구조

```text
Project/
│
├── Dataset/
│   ├── images/
│   ├── catalog_0.catalog
│   ├── catalog_1.catalog
│   └── record_*.json
│
├── UI/
│   ├── Form1.cs
│   ├── Form1.Designer.cs
│   ├── GraphWindow.cs
│   └── Components/
│
├── AI/
│   ├── train.py
│   ├── model/
│   ├── logs/
│   └── dataset/
│
├── Utils/
│
└── README.md
```

---

## 핵심 구현 내용

### 데이터 로드 및 전처리

DonkeyCar에서 수집한 `.catalog` 파일을 분석하여 이미지 경로, 조향값(Angle), 속도값(Throttle)을 추출하고 AI 학습에 사용할 데이터셋을 생성하였습니다.

```python
for rec in records:
    img_path = os.path.join(images_dir, rec.get("cam/image_array", ""))

    img = cv2.imread(img_path)
    img = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
    img = cv2.resize(img, (img_size[1], img_size[0]))
    img = img.astype(np.float32) / 255.0

    X.append(img)
    Y_angle.append(float(rec["user/angle"]))
    Y_throttle.append(float(rec["user/throttle"]))
```

**구현 내용**

* 직진 데이터 필터
* 급커브 데이터 필터
* 주행 여부 필터
* 카탈로그별 데이터 분류
* 범위 선택 삭제 기능
* 다중 프레임 삭제 기능
* Catalog / JSON 동기화 처리
* 삭제 데이터 자동 백업
* 데이터 복원 기능 제공
* 학습 데이터 구성 및 정제 지원

---

### CNN 기반 자율주행 모델 구축

DonkeyCar 구조를 기반으로 CNN 모델을 설계하여 조향각과 속도를 동시에 예측할 수 있도록 구현하였습니다.

```python
x = layers.Conv2D(24, 5, strides=2, activation="relu")(img_in)
x = layers.Conv2D(32, 5, strides=2, activation="relu")(x)
x = layers.Conv2D(64, 5, strides=2, activation="relu")(x)

x = layers.Flatten()(x)
x = layers.Dense(100, activation="relu")(x)

angle_out = layers.Dense(1, activation="tanh", name="n_outputs0")(x)
throttle_out = layers.Dense(1, activation="tanh", name="n_outputs1")(x)
```

**구현 내용**

* CNN 기반 특징 추출
* 조향각(Angle) 예측
* 속도(Throttle) 예측
* Multi Output 모델 구조 적용

---

### 데이터 필터링 및 정제

불필요한 학습 데이터를 제거하여 데이터 품질을 향상시키고 학습 효율을 높일 수 있도록 구현하였습니다.

```csharp
if (cbox1.Checked)
{
    query = query.Where(f => Math.Abs(f.Angle) > 0.05);
}

if (cbox2.Checked)
{
    double curveValue = (double)nud1.Value;
    query = query.Where(f => Math.Abs(f.Angle) >= curveValue);
}
```

**구현 내용**

* 직진 데이터 필터
* 급커브 데이터 필터
* 주행 여부 필터
* 카탈로그별 데이터 분류
* 학습 데이터 품질 향상

---

### 실시간 AI 학습 모니터링

Python 학습 스크립트와 WinForms UI를 연동하여 Epoch별 학습 진행 상황을 실시간으로 확인할 수 있도록 구현하였습니다.

```python
def progress(epoch, total, loss, val_loss):
    print(
        f"[PROGRESS] epoch={epoch}/{total} "
        f"loss={loss:.4f} "
        f"val_loss={val_loss:.4f}",
        flush=True
    )
```

```csharp
if (line.StartsWith("[PROGRESS]"))
{
    txtLog.AppendText(
        $"[학습 {epoch}/{totalEpochs}회] "
        + $"학습오차: {loss:F4} "
        + $"검증오차: {valLoss:F4}"
    );
}
```

**구현 내용**

* Epoch 진행률 표시
* Loss / Validation Loss 실시간 출력
* 학습 그래프 자동 갱신
* 학습 상태 모니터링

---

### 학습 결과 시각화

학습이 종료된 후 Loss와 Validation Loss 그래프를 생성하여 모델 성능을 분석할 수 있도록 구현하였습니다.

```python
plt.plot(epochs_range, loss, label="Train Loss")
plt.plot(epochs_range, val_loss, label="Val Loss")

plt.title("Donkey Car - Training Loss")
plt.xlabel("Epoch")
plt.ylabel("Loss")
```

**구현 내용**

* Loss 그래프 생성
* Validation Loss 그래프 생성
* 과적합 여부 분석
* 최적 모델 성능 검증

---

### AI 모델 검증 및 예측 비교

학습된 모델(.h5)을 불러와 실제 주행 이미지에 대한 조향각을 예측하고,
실제 데이터와 비교 분석할 수 있도록 구현하였습니다.

```python
outputs = model.predict(X, batch_size=64, verbose=0)

angles = outputs[0].flatten()

for img_name, angle in zip(img_names, angles):
    print(f"[PRED] image={img_name} angle={angle:.6f}")
```

**구현 내용**

* 학습된 모델(.h5) 로드
* 이미지 데이터 자동 전처리
* 프레임 단위 조향각 예측
* Actual Steering / Predicted Steering 비교
* 조향각 오버레이 시각화
* 실제 주행 데이터와 예측 결과 비교 분석

 
## 프로젝트 성과

* DonkeyCar 데이터 수집, 정제, 학습 및 검증 과정을 하나의 프로그램으로 통합
* WinForms 기반 GUI를 통한 데이터 탐색 및 관리 환경 구축
* 범위 선택 삭제 및 복원 기능 구현으로 데이터 정제 효율 향상
* Catalog / JSON 동기화 기반 데이터 무결성 유지
* Python TensorFlow 모델과 C# WinForms 프로그램 간 연동 구현
* 실시간 학습 로그 및 Loss 그래프 시각화 기능 제공
* 학습 중지 및 재실행 기능 구현
* 실제 주행 데이터와 AI 예측 결과 비교 분석 기능 구현
* 조향각 오버레이 시각화를 통한 모델 검증 기능 제공
* 데이터 품질 검증부터 모델 성능 분석까지 가능한 통합 학습 환경 구축
* C# 기반 GUI와 Python AI 학습 환경을 연동하여 데이터 관리부터 모델 학습 및 결과 분석까지 하나의 프로그램에서 수행 가능한 환경 구축


---

## 팀 구성

| 이름 | 담당 역할 |
|--------|--------|
| 김근우 | WinForms UI 설계 및 개발 / 데이터 관리 시스템 구현 / Git Branch 관리 및 코드 통합 |
| 김민욱 | 데이터 처리 시스템 개발 / 주행 데이터 필터링 및 전처리 로직 구현 / 데이터 정제 및 품질 관리 기능 개발 |
| 김재민 | CNN 기반 자율주행 AI 모델 설계 / TensorFlow 학습 파이프라인 구축 / 모델 성능 분석 및 검증 시스템 구현 |

---
