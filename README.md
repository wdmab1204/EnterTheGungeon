### 이 문서는 Enter the Gungeon 구현 과정과 시행착오, 코드리뷰와 성능 분석을 기록한 상세 개발 기록입니다

# Enter the Gungeon
![image](https://github.com/user-attachments/assets/37c2967b-158c-4c13-91d2-9ed5a920728f) <br>

몬스터들의 소굴인 던전 속에서 플레이어가 최대한 오래 살아남는 게임입니다.
- 수학적 지식과 통계 이론, 그리고 탐색 알고리즘을 사용해 무작위 던전을 만듭니다.
- 몬스터가 나타나 플레이를 방해하고 보상을 제공합니다.
- 여러 총기가 구현되어있고 플레이어는 다양한 재미를 느낄 수 있습니다.

<br>

개요
---
- 게임 장르 : Roguelike
- 작업 기간 : 2024.10.15 ~ 현재 진행중
- 기술 : C#, Unity Engine, UniTask

목차
---

* [1. 무작위 던전 생성기](#1-무작위-던전-생성기)

  * [1-1. Binary Space Partitioning(BSP) 알고리즘을 이용해 간단한 던전을 구현했습니다.](#1-1-binary-space-partitioningbsp-알고리즘을-이용해-간단한-던전을-구현했습니다)

  * [1-2. Delaunay Triangulation + MST 조합을 채택했습니다.](#1-2-delaunay-triangulation--mst-조합을-채택했습니다)

  * [1-3. 정규 분포를 활용해 방들의 위치를 정했습니다.](#1-3-정규-분포를-활용해-방들의-위치를-정했습니다)

  * [1-4. Convex Hull을 이용해 던전의 외곽을 계산했습니다.](#1-4-convex-hull을-이용해-던전의-외곽을-계산했습니다)

  * [1-5. Graham's scan 방식으로 Convex Hull을 개선했습니다.](#1-5-grahams-scan-방식으로-convex-hull을-개선했습니다)

  * [1-6. 볼록 다각형의 점들을 이용하여 삼각형을 분할했습니다.](#1-6-볼록-다각형의-점들을-이용하여-삼각형을-분할했습니다)

  * [1-7. Delaunay Triangulation을 수행하기 위해 Flip 알고리즘을 채택했습니다.](#1-7-delaunay-triangulation을-수행하기-위해-flip-알고리즘을-채택했습니다)

  * [1-8. 무한 루프에 걸리는 이슈를 수정했습니다.](#1-8-무한-루프에-걸리는-이슈를-수정했습니다)

  * [1-9. Minimum Spanning Tree(MST)를 사용해 간선을 최소화 했습니다.](#1-9-minimum-spanning-treemst를-사용해-간선을-최소화-했습니다)

  <br>

* [2. 던전 복도 로직 설계](#2-던전-복도-로직-설계)

  * [2-1. A star를 이용해 복도를 구현했습니다.](#2-1-a-star를-이용해-복도를-구현했습니다)
  
  * [2-2. 복도의 모양을 최대한 직선을 유지하도록 구현했습니다.](#2-2-복도의-모양을-최대한-직선을-유지하도록-구현했습니다)

  * [2-3. 복도의 두께를 고려하기 위해 던전 전용 그리드를 만들었습니다.](#2-3-복도의-두께를-고려하기-위해-던전-전용-그리드를-만들었습니다)

  <br>

* [3. 미니맵 구현](#3-미니맵-구현)

  * [3-1. Custom UI Texture를 만들어 던전 기반 UI를 만들었습니다.](#3-1-custom-ui-texture를-만들어-던전-기반-ui를-만들었습니다)

  * [3-2. 외곽라인을 추가해 던전의 경계를 명확하게 표시했습니다.](#3-2-외곽라인을-추가해-던전의-경계를-명확하게-표시했습니다)

  * [3-3. 각 타일마다 벡터를 계산하여 외곽라인을 구현했습니다.](#3-3-각-타일마다-벡터를-계산하여-외곽라인을-구현했습니다)

  <br>

* [4. 던전 내비게이션 시스템](#4-던전-내비게이션-시스템)

  * [4-1. A star를 이용해 몬스터와 코인의 이동 경로를 구현했습니다.](#4-1-a-star를-이용해-몬스터와-코인의-이동-경로를-구현했습니다)

  * [4-2. UniTask를 이용해 길 찾기 비용을 분산시켰습니다.](#4-2-unitask를-이용해-길-찾기-비용을-분산시켰습니다)

  * [4-3. 자원을 공유하여 계산이 틀어지는 문제를 수정했습니다.](#4-3-자원을-공유하여-계산이-틀어지는-문제를-수정했습니다)

  * [4-4. 몬스터가 뒤로 가는 문제를 수정했습니다.](#4-4-몬스터가-뒤로-가는-문제를-수정했습니다)

  <br>

* [5. 참고 자료](#5-참고-자료)


<br>
<br>

## 1. 무작위 던전 생성기

<p float="left">
  <img src="https://github.com/user-attachments/assets/4bc42f22-3b55-4508-9cfe-27ca81968ae9" width="80%"><br>
  <sub>실제 동작 화면입니다</sub>
</p>

<br>

### 기능
유저가 던전에 입장할 때마다 **모양과 규칙이 다른 던전을 난수로 생성**

### 효과
매 번 **새로운 던전**을 탐험할 수 있어 높은 리플레이성 제공

<br>

### 1-1. Binary Space Partitioning(BSP) 알고리즘을 이용해 간단한 던전을 구현했습니다.
<p float="left">
  <img width="40%" alt="Image" src="https://github.com/user-attachments/assets/dc41c404-aeb4-420d-ac42-e9ea91c6e422"/>
</p>

무작위 던전 생성을 위해 예제 코드가 풍부하고 구현이 단순한 BSP 알고리즘을 우선 적용했습니다.

BSP는 방과 복도를 손쉽게 정의할 수 있어 빠른 프로토타이핑에 적합하다고 생각했으나 예상 밖의 문제들이 발생했습니다.

문제점
- 방의 크기와 배치가 지나치게 규칙적
- 복도가 방과 다른 복도를 가로지르는 문제
- 인위적인 격자 구조에 가까운 모습

로그라이크 장르가 요구하는 무작위성과 탐험의 재미를 충분히 살리기에는 부족했습니다.

<br>

### 1-2. Delaunay Triangulation + MST 조합을 채택했습니다.

<img width="40%" alt="image" src="https://github.com/user-attachments/assets/4447b641-5b4f-43ee-9469-27cd51ced40a" />

무작위 던전 생성을 구현하기 위해 다양한 알고리즘들이 존재하지만, 본 프로젝트에서는 자연스럽고 탐험 욕구를 자극하는 레이아웃을 만드는 것을 목표로 하고 있습니다.

이를 위해 자료조사 중 Delaunay Triangulation + MST를 조합한 방식을 발견하였고 실제 적용 모습이 목표로 하고자 하는 모습과 유사하여 이 방법을 채택했습니다.

던전 구조를 그래프로 해석하면 각 방은 노드이고 방을 잇는 복도는 간선 역할을 합니다.
- Delaunay Triangulation을 통해 인접 방들 간의 합리적인 연결 관계 형성
- MST를 적용하여 불필요한 Cycle을 제거함으로 써 모든 방을 방문 가능한 최소 경로만 남김

이를 통해 복잡성을 줄이면서도 플레이어가 던전을 자연스럽게 탐험할 수 있는 구조를 만들 수 있을거라 생각했습니다.

아이디어와 알고리즘 이론 지식은 유튜브 및 웹 자료[^1]에서 많은 도움을 받았습니다.

<br>


### 1-3. 정규 분포를 활용해 방들의 위치를 정했습니다.

<img width="70%"  alt="image" src="https://github.com/user-attachments/assets/bbf5e235-2656-4d92-9d72-fd069e3642ea" />

처음에는 난수 생성기(System.Random)를 이용해 방 위치를 무작위로 배치하여 던전을 구현했습니다.

그러나 직접 플레이해보니, 방 간 거리가 지나치게 멀어 이동 시 지루함이 발생하는 문제가 있었습니다.

이를 개선하기 위해 방들의 위치를 최대한 근접하게 배치하고 무작위성을 유지할 수 있는 방법을 고민했고, 정규 분포를 활용하는 아이디어를 적용했습니다. 

난수 생성기 대신 정규 분포를 사용함으로써, 방들이 자연스럽게 군집을 이루면서도 무작위로 배치되도록 구현할 수 있었습니다.

그 결과, 플레이어가 탐험할 때 이동 거리가 적절하게 분포되어 방 이동 간 지루함을 덜어냈습니다.

<br>

### 1-4. Convex Hull을 이용해 던전의 외곽을 계산했습니다.

<p float="left">
  <img width="40%" alt="image" src="https://github.com/user-attachments/assets/aa8fb8af-2bf4-4d69-8e1d-cb6700406739" /><br>
  <sub>Convex Hull을 이용해 정점들을 Line Renderer로 이은 모습</sub>
</p>


무작위 던전의 방 위치는 사전에 이미 결정되어 있어 삼각분할 과정에서 점이 추가되는 Incremental 방식과 적합하지 않았습니다.
 
이 방식은 구현 편의성을 위해 보편적으로 Super Triangle을 사용해 점들을 감싸지만, 이는 점을 하나씩 추가하며 삼각분할을 확장할 때 유용합니다.

하지만 본 프로젝트는 모든 방의 위치가 이미 정해져 있고 삼각분할 후 외곽이 항상 볼록하다는 점을 활용해서 던전 외곽을 계산할 때 Convex Hull을 적용하여 삼각분할 초기 작업을 단순화 했습니다.

<br>

### 1-5. Graham's scan 방식으로 Convex Hull을 개선했습니다.

처음에는 임의의 두 점을 연결하는 직선을 기준으로 정하고 모든 점들이 한쪽으로 치우쳐져 있으면 외곽선으로 판단하는 Brute Force 방식을 생각했으나 N이 정점일 때 총 선분의 개수는 n(n-1)/2 즉 O(N^2)이고 동시에 모든 점을 순회해서 확인해야하니 추가로 O(N)이 발생하여 즉 O(N^2) * O(N) = O(N^3)이라는 Time Complexity가 발생합니다.

매우 비효율적인 방법이기 때문에 개선안을 위한 자료 조사중 널리 알려지고 검증된 알고리즘인 Graham's scan을 채택했습니다.

이 방법은 한 점을 기준으로 각도 순으로 정렬한 후 스택을 이용해 외곽을 형성하는 점들을 구하는 방식으로 O(n log n) Time Complexity가 발생합니다.

```csharp
sortedList.Sort((v1, v2) =>
{
    var ccw = MathUtility.IsAPointLeftOfVectorOrOnTheLine(v1.GetPos2D_XY(), v2.GetPos2D_XY(), minVertex.GetPos2D_XY());
    if (Mathf.Abs(ccw) < Mathf.Epsilon)
        return Vector3.Distance(v1.position, v2.position) < 0 ? 1 : -1;
    else
        return ccw < 0f ? 1 : -1;
});
```

```csharp
public static bool IsTriangleOrientedClockwise(Vector2 p1, Vector2 p2, Vector2 p3)
{
    bool isClockWise = true;

    float determinant = p1.x * p2.y + p3.x * p1.y + p2.x * p3.y - p1.x * p3.y - p3.x * p2.y - p2.x * p1.y;

    if (determinant > 0f)
    {
        isClockWise = false;
    }

    return isClockWise; 
}
```

<br>

가장 왼쪽 하단의 점(P)을 기준으로 모든 점과 잇는 선분이 X축을 기준으로 이루는 극각을 오름차순으로 정렬해야하는데 임의의 점 A와 B를 놓고 벡터 PA와 PB의 외적을 이용하면 점P가 벡터AB의 왼쪽에 있는지 판단할 수 있습니다.
 
정렬이 완료된 후에는 스택을 사용하여 최종 볼록 껍질을 이룰 선분들을 걸러냈습니다.
 
그리고 새로운 점을 순회할 때마다 스택에 쌓인 최근 두 점과 함께 반시계 방향을 이루는지 행렬식[^2]을 이용해 검사했습니다. 

만약 일직선이거나 오목할 경우 해당 점을 제외하고 다시 검사하는 방법으로 구현했습니다.

<br>

<p float="left">
  <img src="https://github.com/user-attachments/assets/cf8543e3-3f13-4f8e-9725-6efe40d85c41" width="40%"/>
  <img src="https://github.com/user-attachments/assets/7dfcfe6e-e02a-4e28-beb0-70cf74f2f788" width="40%" />
</p>

### 1-6. 볼록 다각형의 점들을 이용하여 삼각형을 분할했습니다.

첫번째로 다각형의 한 점을 기준으로 잡고, 나머지 모든 점들과 선분을 연결하여 삼각형을 형성했습니다.

간단하지만 이 방식이 가능한 이유는 *볼록 다각형(convex polygon)* 이기 때문입니다. 만약 오목한 부분이 존재한다면 분할 과정에서 생성된 선분이 교차하게 되어 올바른 삼각형 분할 성립이 어렵습니다.

두번째로 다각형 내부에 위치한 나머지 점들을 순회하여 만들어진 삼각형 내부에 포함되는지 판별했습니다.

특정 점이 삼각형 내부에 포함되는지 여부는 행렬식[^3]을 이용하여 판별했습니다.

<br>

### 1-7. Delaunay Triangulation을 수행하기 위해 Flip 알고리즘을 채택했습니다.

<img width="580" height="598" alt="image" src="https://github.com/user-attachments/assets/0f7d3e6a-a697-4eaa-b65c-6f4c4bbe6b70" />

Delaunay Triangulation을 구현하는 방식은 여러가지가 있으며, 대표적으로 Incremental 알고리즘이 있습니다.

 이는 점이 동적으로 추가되는 상황에 적합하지만 본 프로젝트는 이미 Convex Hull을 기반으로 삼각형들을 생성해둔 상태였기 때문에, 기존 삼각형들을 조건에 맞게 재구성할 수 있는 Flip 알고리즘을 채택했습니다.

//이미지 필요<br>
Flip 조건을 달성하기 위해 아래와 같은 조건이 필요합니다.

- 마주보는 두 삼각형의 외접원안에 상대 삼각형의 점이 포함되지 않아야 한다.
- 두 삼각형이 이루는 도형이 반드시 볼록 사각형이여야 한다.

위 조건들은 행렬식[^5]을 이용해 쉽게 구할 수 있었습니다.


<br>

### 1-8. 무한 루프에 걸리는 이슈를 수정했습니다.

```csharp
//aPos, bPos, cPos를 점으로 가진 삼각형의 외접원 안에 dPos가 포함되는지 확인
if (GetDeterminantForCircumCircleInPoint(aPos, bPos, cPos, dPos) < 0f)
{
    if (IsQuadrilateralConvex(aPos, bPos, cPos, dPos))
    {
        if (GetDeterminantForCircumCircleInPoint(bPos, cPos, dPos, aPos) < 0f)
            continue;

        hasFlippedEdge = true;
        FlipEdge(thisEdge);
    }
}
```

Flip을 이론대로 알고리즘을 수행했음에도 불구하고 점이 여전이 외접원 안에 포함되는 경우가 존재했습니다. 이는 Flip을 수행해도 더 이상 개선되지 않는 특정 경우의 수 때문이였습니다.

무한 루프를 방지하기 위해, Flip 연산을 수행하기 전에 사전 검증 단계를 추가했습니다.

Flip을 가정한 새로운 삼각형(예시 코드 기준 bPos, cPos, dPos가 점인 삼각형)의 외접원을 미리 유추해 외접원에 포함되는지 사전에 판정합니다.

조건을 충족하지 않는 경우에는 Flip 연산을 중지하여 무한 루프 발생 가능성을 제거했습니다.

<br>

### 1-9. Minimum Spanning Tree(MST)를 사용해 간선을 최소화 했습니다.

<img width="50%" alt="image" src="https://github.com/user-attachments/assets/f6935ae9-4cb7-4064-ba66-2011b32b2e6d" />

 
모든 방들을 방문할 수 있지만 같은 방을 다시 방문할 수 있는 루트(Cycle)가 존재합니다. 던전이 너무 복잡해지지 않기 위해 최소 신장 트리를 적용하여 최소한의 루트만 남깁니다.

들로네 삼각분할로 생성된 그래프에는 Cycle(순환 경로)가 존재합니다. 

초기 구현에서도 복도가 방을 가로지르는 현상으로 인해 순환 경로가 있었고 현재도 존재하여 불필요하게 복잡하다는 문제점이 여전히 존재합니다.

Tree의 특징 중 하나인 *순회하지 않는다*는 점을 이용하여 Delaunay Triangulation을 이용해 만든 그래프를 Tree로 변환해주는 Minimum Spanning Tree라는 알고리즘을 채택했습니다.

현재는 방과 방의 거리를 가중치로 설정하여 게임에 적용된 상태이지만 추후 간선 설계를 의도적으로 바꾸고 싶을 때 가중치를 바꾸는 방법 등으로 인위적으로 조작이 가능하도록 아키텍쳐를 설계했습니다.





## 2. 던전 복도 로직 설계

<img width="50%" alt="image" src="https://github.com/user-attachments/assets/690a7572-f691-4f09-b74d-4c7d2e0bd1de" />

### 2-1. A star를 이용해 복도를 구현했습니다.

초기 구현방식은 방과 방을 단순히 직선 혹은 ㄴ자 형태로 생성했습니다. 그러나 이 방식은 다른 방을 가로지르는 문제가 발생하여 그래프 데이터의 일관성이 깨지는 문제가 있었습니다.

이 문제를 해결하기 위해 A star 알고리즘을 도입하여 타일맵의 타일들을 노드로 정의하고 두 방 사이의 최단 경로 데이터를 기반으로 타일을 추가로 그려 복도를 생성했습니다.

이로써 기존 복도가 다른 타일들을 무분별하게 관통하는 문제가 해결되고 자연스러운 구조적 복도 배치를 위한 아키텍쳐를 구현할 수 있었습니다.

<br>

### 2-2. 복도의 모양을 최대한 직선을 유지하도록 구현했습니다.

최단거리를 기준으로 탐색하다 보니 복도의 경로가 대각선으로 형성되는 경우가 많았습니다. 

이로인해 플레이어는 이동 중 빈번하게 방향 전환을 해야 했고 실제 플레이를 해 보니 손가락에 피로도가 누적되는 문제가 발생했습니다. 

기존 휴리스틱 함수는 가장 빠른 최단의 거리만을 고려했기 때문에 타일 위주의 맵에서 턴이 많이 발생하는게 원인이였습니다. 

이를 해결하기 위해 A star 휴리스틱 함수에 방향 전환 시 추가 가중치를 부여하도록 비용을 추가하여 턴 개수를 최소화하는 경로가 나오도록 개선했습니다.

<br>

### 2-3. 복도의 두께를 고려하기 위해 던전 전용 그리드를 만들었습니다.

복도를 일정 두께로 표현하고 벽을 세우는 과정에서 경로 주변 타일과 겹치는 현상이 발생했습니다.

이는 초기 A star 계산에서 *복도의 두께*를 고려하지 않았기 때문입니다.
 
노드 단위를 단일 타일(1x1)로 정의를 하였고 경로 계산에 문제는 없으나 복도의 두께까지 반영하려면 각 노드 반경 검증이 추가로 필요해 탐색 비용을 크게 증가시켜 성능 저하로 이어집니다.
  
그래서 복도 두께 문제 해결 및 연산량 개선을 위해 던전 전용 그리드를 도입하여 5x5 타일을 하나의 노드로 재정의하였고 노드안에서 복도의 표현을 자유롭게 할 수 있기 때문에 원인을 해결할 수 있게 되었습니다.

추가로 노드의 개수도 기존보다 25배 가량 감소되었기 때문에 연산량도 크게 개선되었습니다.

<br>

### 최종 모습
<p float="left">
  <img width="50%" alt="image" src="https://github.com/user-attachments/assets/4243eae5-e17f-4603-be79-6cd003eee283" />
</p>


<br>

### 성능 테스트

|  | 방 10개 | 방50개 | 방 100개 |
| :-:  | :-: | :-: | :-:|
| 소요시간(최저) | 0.03s | 0.20s | 0.41s |
| 소요시간(최고) | 0.05s | 0.23s | 0.58s |
| 소요시간(평균) | 0.04s | 0.23s | 0.45s |

<br>

## 3. 미니맵 구현

![Recording 2025-08-30 at 20 55 55](https://github.com/user-attachments/assets/7f9b1c37-c952-4372-a793-7797c4da0d39)

기능 : 유저가 입장한 던전의 모습을 한눈에 볼 수 있는 UI<br>
효과 : 복잡한 던전에서 길을 헤메지 않아 쉽게 게임 플레이 가능

### 3-1. Custom UI Texture를 만들어 던전 기반 UI를 만들었습니다.

방들의 타일 정보와 A star 알고리즘으로 생성한 복도 데이터를 이용하여 전체 맵을 렌더링했습니다.

방 모양 렌더링은 각 방의 타일 정보를 불러와 하나의 Vertex로 간주하고 인덱스를 기반으로 Triangle을 구성하여 하나의 Mesh 객체로 렌더링했습니다.

복도 렌더링은 A star로 얻은 Waypoint 경로를 기반으로 Triangle strip으로 변환하였고 Segment기능도 추가해 복도의 두께도 조절할 수 있는 Mesh를 만들었습니다.

<br>

### 3-2. 외곽라인을 추가해 던전의 경계를 명확하게 표시했습니다.

하얀색으로만 그려진 던전의 모습은 너무 단조로워 벽이나 장애물이 설치된 타일은 검은색으로 칠해 보다 많은 정보를 제공하고 싶었습니다.

외곽 라인을 얻기 위해서 경계선을 따라 순회해야 했기 때문에 시작점에서 출발하여 인접한 타일을 따라가다 다시 출발점에 도달하면 폐곡선을 만드는 방법을 생각했습니다.

하지만 이 방법은 외곽 라인이 여러개(M) 만들어져야 할 경우 여러번 순회해야하기 때문에 O(N * M)이라는 Time Complexicy가 발생합니다.

외곽 라인 개수가 적다면 문제가 없지만 만약 많다면 미니맵 UI를 열 때 프레임 드랍이 발생할 것을 고려하여 더 좋은 방법을 찾기 위해 자료 조사를 하였습니다.

<br>

### 3-3. 각 타일마다 벡터를 계산하여 외곽라인을 구현했습니다.

<img width="50%" alt="image" src="https://github.com/user-attachments/assets/5d3c2769-0437-4874-861a-7feda70fbd24" />


자료 조사 중 한 유튜브 영상[^6]에서 아이디어를 얻었습니다.

각 타일마다 시계방향 순서의 4개 위치 벡터를 저장고 모든 타일마다 반복하는데 만약 겹치는 벡터가 있다면 제거를 하는 방법입니다.

이렇게 하면 최종적으로 남은 벡터의 집합은 시계방향 벡터와 반시계방향 벡터로 구분됩니다.

본 프로젝트에서는 모두 검은색 선으로 표기하였으나 이를 응용해서 외벽과 내벽의 스타일을 다르게 하는것도 가능합니다.

<br>

## 4. 던전 내비게이션 시스템

![Recording 2025-08-30 at 21 09 45](https://github.com/user-attachments/assets/31f5c93a-6540-4d0c-924b-521f18aa3c59)

### 4-1. A star를 이용해 몬스터와 코인의 이동 경로를 구현했습니다.

A star는 게임 알고리즘 중에 길찾기로 가장 유명한 알고리즘이기 때문에 깊은 고민 없이 적용했지만 문제가 발생했습니다.

생각보다 한 프레임에 사용된 CPU 사용량이 적지 않다는 점을 Unity Profiler를 이용해 발견했습니다.

경로 업데이트 주기를 0.3초로 늘리고 의미없는 객체 할당량을 줄여 GC 이슈도 최소화 하였으나 CPU 사용량은 여전했습니다.

원인을 분석해보니 여러 몬스터가 한 번에 많은 경로 요청을 했고 생각보다 많은 연산이 발생한 걸로 파악됐습니다.

|  | A star | Unity NavMesh |
| :-:  | :-: | :-: |
| Call| 8 | 8 |
| 평균 CPU 점유율 | 49% | 0.8% |
| GC Allock | 137KB | 0B |
| Time ms | 1.34 | 0.01 |
| FPS .avg | 350 | 750 |

<br>

### 4-2. UniTask를 이용해 길 찾기 비용을 분산시켰습니다.

한 프레임에 과한 CPU 사용량은 프레임 드랍을 유발하기 때문에 고치기 위한 여러 방법들을 생각했습니다.

처음에는 Queue를 사용해 경로 요청을 프레임 마다 순차적으로 처리하는 방법을 생각했습니다.

한 프레임에 모든 경로 요청을 수행하는 것 보다 여러 프레임으로 각 요청을 수행하는 게 확실히 더 나은 방법이나 요청량이 많거나 계산이 오래 걸릴 경우 병목 현상이 발생하고 실시간으로 경로를 업데이트해야하는 게임 특성 상 적합하지 않다고 판단했습니다.

두번째는 Thread를 사용해 탐색 연산을 메인 스레드와 분리하여 수행하는 방식입니다. 

경로 연산을 병렬화 하여 프레임에 비용이 과도하게 몰리는 걸 방지하여 프레임 드랍을 예방할 수 있으나 본 프로젝트의 목표인 WebGL 환경에서 Thread를 지원하지 않는 치명적 제약이 있어 사용할 수 없었습니다.

최종적으로 UniTask를 선택했습니다.

경로 계산을 여러 프레임으로 나누어 연산하면 한 프레임에 집중되는 연산량을 줄이고 프레임 드랍을 방지할 수 있습니다. 코루틴 방식과 매우 유사하지만 try catch문을 사용할 수 있고 가비지 생성 부담을 줄일 수 있는 이점이 있습니다.

<br>

### 4-3. 자원을 공유하여 계산이 틀어지는 문제를 수정했습니다.

경로 탐색 과정에서 던전의 길찾기 전용 Grid 데이터를 참조하는데 기존에는 경로 계산을 마치면 모든 노드를 초기화했습니다.

하지만 UniTask 방식을 채택하고 여러 경로 요청이 들어오면 계산도 동시에 이루어지는데 참조형 데이터를 공유하면서 계산 충돌이 발생했습니다.

이것은 참조형 데이터가 동일한 객체를 공유하는 특성이기 때문에 발생하는 문제였습니다.

이를 해결하기 위해 경로 계산 중 노드를 꺼낼 때 *값 타입(struct)*으로 변환하여 사용하였습니다. 이는 스택에 복사되어 독립적으로 연산되므로 참조 충돌 문제를 방지할 수 있었습니다.

아래는 성능 분석 표입니다, 여러 프레임에 나눠서 계산하고 UniTask는 Unity Profiler와 호환되지 않기 때문에 정확한 성능 분석은 어려웠으나 경로 계산 속도가 빨라졌고 프레임 드랍이 더이상 일어나지 않았습니다.

|  | 기존 A star | UniTask + A star |
| :-:  | :-: | :-: |
| Time ms | 1.34 | 0.66 |
| FPS .avg | 350 | 723 |

<br>

### 4-4. 몬스터가 뒤로 가는 문제를 수정했습니다.

<p float="left">
  <img src="https://github.com/user-attachments/assets/15795f38-583d-492d-b5ea-bc0ff6b2d8b8" width="120" height="100" style="object-fit:cover;"/>
  <img src="https://github.com/user-attachments/assets/478db38c-bc51-45d3-90ca-9e217731c0ef" width="120" height="100" style="object-fit:cover;"/>
</p>

###### (좌) 몬스터가 뒤로 가는 현상 &nbsp;&nbsp; | &nbsp;&nbsp; (우) 내적을 이용해 수정한 모습


UniTask로 변경 후 몬스터가 일정 초 간격으로 경로의 반대방향으로 이동해 부자연스러운 움직임을 보였습니다.

이는 경로 요청 시점의 몬스터 위치와 실제 경로 계산 완료 시점의 위치가 불일치하여, 몬스터가 과거 위치를 기준으로 경로를 따르면서 나타난 현상이었습니다.

웨이포인트 중 현재 진행 방향과 반대에 위치한 지점을 무시하도록 로직을 수정하였습니다.

몬스터의 실제 이동 벡터와 다음 웨이포인트 방향 벡터 간의 내적 연산을 적용하여, 진행 방향과 일치하지 않는 경우 해당 웨이포인트를 제외하도록 구현하였습니다.

그 결과, 몬스터의 움직임이 자연스러워지고 불필요한 방향 전환으로 인한 이질감이 해소되었습니다.

## 5. 참고 자료
**아이디어**
- [Delaunay Triangulation + MST 아이디어 참고 자료](https://www.youtube.com/watch?v=rBY2Dzej03A)
- [외곽선 구하는 방법](https://www.youtube.com/watch?v=ku_thRxLXPw)

**이론 학습 자료**
- [Delaunay Triangulation 이론 학습 자료](https://en.wikipedia.org/wiki/Delaunay_triangulation)
- [Flip 이론 학습 자료(8페이지 참고)](https://juliageometry.github.io/DelaunayTriangulation.jl/stable/tutorials/operations_flip_edge/)
- [MST 이론 학습자료](https://4legs-study.tistory.com/111)

**행렬식 참고 자료**
- [점 3개의 시계방향을 확인하는 행렬식](https://en.wikipedia.org/wiki/Curve_orientation)
- [점이 삼각형안에 포함되는지 확인하는 행렬식](http://totologic.blogspot.com/2014/01/accurate-point-in-triangle-test.html)
- [점이 원 안에 포함되는지 확인하는 행렬식](https://math.stackexchange.com/questions/3965500/understand-the-relation-between-orientation-in-circle-test-and-determinant)
- [볼록 사각형인지 확인하는 방법](https://stackoverflow.com/questions/2122305/convex-hull-of-4-points)

<br>

긴 글 읽어주셔서 감사합니다.

[^1]: [Delaunay Triangulation + MST 아이디어 참고 자료](https://www.youtube.com/watch?v=rBY2Dzej03A)<br>
[Delaunay Triangulation 이론 학습 자료](https://en.wikipedia.org/wiki/Delaunay_triangulation)<br>
[Flip 이론 학습 자료(8페이지 참고)](https://juliageometry.github.io/DelaunayTriangulation.jl/stable/tutorials/operations_flip_edge/)<br>
[MST 이론 학습자료](https://4legs-study.tistory.com/111)

[^2]: [점 3개의 시계방향을 확인하는 행렬식](https://en.wikipedia.org/wiki/Curve_orientation)

[^3]: [점이 삼각형안에 포함되는지 확인하는 행렬식](http://totologic.blogspot.com/2014/01/accurate-point-in-triangle-test.html)

[^4]: [점이 원 안에 포함되는지 확인하는 행렬식](https://math.stackexchange.com/questions/3965500/understand-the-relation-between-orientation-in-circle-test-and-determinant)

[^5]: [볼록 사각형인지 확인하는 방법](https://stackoverflow.com/questions/2122305/convex-hull-of-4-points)

[^6]: [외곽선 구하는 방법](https://youtu.be/ku_thRxLXPw?si=VIaVR-vv4oRQw2iT&t=114)
