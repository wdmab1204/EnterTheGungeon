### 이 문서는 Enter the Gungeon 구현 과정과 시행착오, 코드리뷰와 성능 분석을 기록한 상세 개발 기록입니다

Enter the Gungeon
---
![image](https://github.com/user-attachments/assets/37c2967b-158c-4c13-91d2-9ed5a920728f) <br>

몬스터들의 소굴인 던전 속에서 플레이어가 최대한 오래 살아남는 게임입니다.
- 수학적 지식과 통계 이론, 그리고 탐색 알고리즘을 사용해 던전 플레이를 제공합니다.
- 몬스터가 나타나 플레이를 방해하고 보상을 제공합니다.
- 여러 총기가 구현되어있고 플레이어는 다양한 재미를 느낄 수 있습니다.
- 운이 좋으면 보물 상자를 발견할 수 있습니다.

개요
---
- 게임 장르 : Roguelike
- 작업 기간 : 2024.10.15 ~ 현재 진행중
- 기술 : C#, Unity Engine, UniTask



목차
---
* 무작위 던전 생성기
* 미니맵 구현
* 던전 내비게이션 시스템

//행렬식 직접 작성하기

무작위 던전 생성기 [[코드 링크]](https://github.com/wdmab1204/EnterTheGungeon/blob/main/Client/Assets/Scripts/DungeonGenerator)
---
![Recording 2025-08-30 at 20 38 27](https://github.com/user-attachments/assets/4bc42f22-3b55-4508-9cfe-27ca81968ae9)

기능 : 유저가 던전에 입장할 때 마다 모양과 규칙이 다른 던전을 난수로 만들어내는 기능

효과 : 유저가 매 번 새로운 던전을 탐험

### 초기 구현 과정
<img width="760" height="724" alt="Image" src="https://github.com/user-attachments/assets/dc41c404-aeb4-420d-ac42-e9ea91c6e422"/>

예제 코드가 많고 보편적으로 사용되는 Binary Space Partitioning (BSP) 알고리즘을 적용하여 무작위 던전을 생성했습니다. BSP는 구현이 간단하고 방과 복도를 쉽게 정의할 수 있다는 장점이 있어 빠르게 프로토타입을 제작하기에 적합했습니다.

그러나 결과물은 방의 크기와 배치가 지나치게 규칙적이고, 복도가 직선적으로 연결되는 경향이 강해 자연스러운 레이아웃보다는 인위적인 격자 구조에 가까웠습니다. 특히 로그라이크 장르가 요구하는 무작위성과 탐험의 재미를 살리기에는 부족하다고 판단했습니다.

또한 방을 직선으로 연결하는 과정에서 다른 방을 가로지르는 현상까지 발생하여, 구조적으로 개선이 필요하다고 느꼈습니다.

### 개선안
<img width="788" height="830" alt="image" src="https://github.com/user-attachments/assets/4447b641-5b4f-43ee-9469-27cd51ced40a" />

###### 실제 구현 화면입니다

Delaunay Trinagulation과 MST를 조합한 방식을 채택했습니다. 무작위 던전을 생성하는 알고리즘은 다수 존재하지만 최대한 자연스럽고 탐험 욕구가 드러나는 형태를 만들고 싶었습니다. 

이 알고리즘의 결과물을 그래프로써 바라본다면 방을 잇는 간선의 역할이 분명하고 MST를 사용하여 노드를 순회하는 경우의 수를 제거할 수 있습니다. 

해당 구현은 유튜브 영상의 설명을 참고했으며, 아이디어와 알고리즘의 이론적 배경은 웹페이지[^1]에서 큰 도움을 받았습니다.<br>


### 정규분포를 활용한 방 위치 배정
<img width="1453" height="821" alt="image" src="https://github.com/user-attachments/assets/bbf5e235-2656-4d92-9d72-fd069e3642ea" />

처음에는 난수 생성기(System.Random)를 이용해 방 위치를 무작위로 배치하여 던전을 구현했습니다. 그러나 직접 플레이해보니, 방 간 거리가 지나치게 멀어 이동 시 지루함이 발생하는 문제가 있었습니다.

이를 개선하기 위해 방들의 위치를 최대한 근접하게 배치하고 무작위성을 유지할 수 있는 방법을 고민했고, 정규 분포를 활용하는 아이디어를 적용했습니다. 난수 생성기 대신 정규 분포를 사용함으로써, 방들이 자연스럽게 군집을 이루면서도 무작위로 배치되도록 구현할 수 있었습니다.

그 결과, 플레이어가 탐험할 때 이동 거리가 적절하게 분포되어 방 이동 간 지루함을 덜어냈습니다.

### Delaunay Triangulation

<img width="595" height="613" alt="image" src="https://github.com/user-attachments/assets/aa8fb8af-2bf4-4d69-8e1d-cb6700406739" />

Convex Hull을 이용해 던전의 외곽을 계산했습니다.

무작위 던전의 방 위치는 사전에 이미 결정되어 있어 삼각분할 과정에서 점이 추가되는 Incremental 방식과 적합하지 않았습니다.
 
이 방식은 구현 편의성을 위해 보편적으로 Super Triangle을 사용해 점들을 감싸지만, 이는 점을 하나씩 추가하며 삼각분할을 확장할 때 유용합니다.

하지만 본 프로젝트는 모든 방의 위치가 이미 정해져 있고 삼각분할 후 외곽이 항상 볼록하다는 점을 활용해서 던전 외곽을 계산할 때 Convex Hull을 적용하여 삼각분할 초기 작업을 단순화 했습니다.

Graham's scan 방식으로 Convex Hull을 구현했습니다. 

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

외적을 이용해 Graham's scan을 구현했습니다. 

가장 왼쪽 하단의 점(P)을 기준으로 모든 점과 잇는 선분이 X축을 기준으로 이루는 극각을 오름차순으로 정렬해야하는데 임의의 점 A와 B를 놓고 벡터 PA와 PB의 외적을 이용하면 점P가 벡터AB의 왼쪽에 있는지 판단할 수 있습니다.

이를 이용하여 정렬한 후 스택을 이용해 점 3개가 이루는 반시계 방향을 판단해서 최종적으로 외곽 점들만 걸러낼 수 있었습니다.
<br>

외적과 행렬식[^2]을 이용해 Graham's scan을 구현했습니다.

det = $\begin{bmatrix}x1 &y1 & 1\\x2 & y2 & 1 \\x3 & y3 & 1 \end{bmatrix}$;





가장 왼쪽 하단의 점(P)을 기준으로 모든 점과 잇는 선분이 X축을 기준으로 이루는 극각을 오름차순으로 정렬해야하는데 임의의 점 A와 B를 놓고 벡터 PA와 PB의 외적을 이용하면 점P가 벡터AB의 왼쪽에 있는지 판단할 수 있습니다.
 
정렬이 완료된 후에는 스택을 사용하여 최종 볼록 껍질을 이룰 선분들을 걸러냈습니다.
 
그리고 새로운 점을 순회할 때마다 스택에 쌓인 최근 두 점과 함께 반시계 방향을 이루는지 검사했습니다. 만약 일직선이거나 오목할 경우 해당 점을 제외하고 다시 검사하는 방법으로 구현했습니다.

<br>
<br>
<br>

<p float="left">
  <img src="https://github.com/user-attachments/assets/cf8543e3-3f13-4f8e-9725-6efe40d85c41" width="40%"/>
  <img src="https://github.com/user-attachments/assets/7dfcfe6e-e02a-4e28-beb0-70cf74f2f788" width="40%" />
</p>

볼록 다각형의 점들을 이용하여 삼각형을 분할했습니다.

첫번째로 다각형의 한 점을 기준으로 잡고, 나머지 모든 점들과 선분을 연결하여 삼각형을 형성했습니다.

간단하지만 이 방식이 가능한 이유는 *볼록 다각형(convex polygon)* 이기 때문입니다. 만약 오목한 부분이 존재한다면 분할 과정에서 생성된 선분이 교차하게 되어 올바른 삼각형 분할 성립이 어렵습니다.

두번째로 다각형 내부에 위치한 나머지 점들을 순회하여 만들어진 삼각형 내부에 포함되는지 판별했습니다.

특정 점이 삼각형 내부에 포함되는지 여부는 행렬식[^3]을 이용하여 판별했습니다.



<img width="580" height="598" alt="image" src="https://github.com/user-attachments/assets/0f7d3e6a-a697-4eaa-b65c-6f4c4bbe6b70" />



Delaunay Triangulation을 수행하기 위해 Flip 알고리즘을 채용했습니다. 

Delaunay Triangulation을 구현하는 방식은 여러가지가 있으며, 대표적으로 Incremental 알고리즘이 있습니다. 이는 점이 동적으로 추가되는 상황에 적합하지만 본 프로젝트는 이미 Convex Hull을 기반으로 삼각형들을 생성해둔 상태였기 때문에, 기존 삼각형들을 조건에 맞게 재구성할 수 있는 Flip 알고리즘을 채택했습니다.

//이미지 필요<br>
Flip 조건을 달성하기 위해 아래와 같은 조건이 필요합니다.

- 마주보는 두 삼각형의 외접원안에 상대 삼각형의 점이 포함되지 않아야 한다.
- 두 삼각형이 이루는 도형이 반드시 볼록 사각형이여야 한다.

위 조건들은 행렬식[^5]을 이용해 쉽게 구할 수 있었습니다.


```csharp
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

무한루프에 걸리는 이슈를 수정했습니다. Flip을 수행했음에도 불구하고 점이 여전히 외접원에 포함되는 경우가 존재했기 때문입니다. 이는 Flip 연산 자체로는 더 나아지지 않는 경우의 수가 있기 때문에 발생했습니다.

해결을 위해 Flip을 수행하기 전 단계에서 Flip 이후의 삼각형을 미리 유추하고, 해당 외접원을 계산해 조건을 사전에 검증하는 방식을 도입했습니다. 이러면 미리 외접원 포함 유무를 알아낼 수 있어 무한루프에 빠지지 않습니다.

### Minimum Spanning Tree

<img width="540" height="580" alt="image" src="https://github.com/user-attachments/assets/f6935ae9-4cb7-4064-ba66-2011b32b2e6d" />

 
#### 모든 방들을 방문할 수 있지만 같은 방을 다시 방문할 수 있는 루트(Cycle)가 존재합니다. 던전이 너무 복잡해지지 않기 위해 최소 신장 트리를 적용하여 최소한의 루트만 남깁니다.

### 던전 전용 그리드 구현 및 A star를 이용한 복도 구현

<img width="595" height="597" alt="image" src="https://github.com/user-attachments/assets/690a7572-f691-4f09-b74d-4c7d2e0bd1de" />

#### 개선하기 전에 방과 방을 잇는 복도는 직선으로 구현하거나 ㄴ자 모양으로 나오도록 구현했습니다. 하지만 복도가 다른 방을 가로지르는 문제가 있었기 때문에 길찾기 알고리즘을 이용해서 던전의 구조를 파악하고 최적의 복도(경로)를 만드는 작업을 진행했습니다.

#### 처음에는 타일을 하나의 노드로 정의하고 A star로 방과 방을 잇는 최단 거리의 경로로 복도를 구현했습니다. 하지만 복도를 두껍게하고 벽을 세우는 과정에서 다시한번 방과 겹치는 이슈가 발생했는데 경로를 계산하는 과정에서 복도의 두께까지 가정하지 않았기 때문입니다. 노드마다 복도의 두께만큼 반경에 다른 타일이 있는지 검사하는 방법이 있지만 안그래도 많은 노드 개수에 추가 비용이 드는건 좋지 않다고 생각해 5x5 타일들을 하나의 노드로 재정의하는 방법을 채용했습니다. 5x5 타일 내에서 복도를 자유롭게 표현할 수 있고 노드의 크기도 커져 전체적인 노드 개수도 줄어들어 A*의 연산 속도고 개선되는 일석이조의 효과를 얻었습니다.

처음에는 방과 방을 잇는 복도를 단순 직선 혹은 ㄴ자 형태로 구현했습니다. 그러나 이 방식은 다른 방을 가로지르는 문제가 있었습니다.

A*를 이용해 가로지르는 문제를 해결했습니다. 타일맵의 각 타일을 노드로 정의하고 두 방 사이의 최단 경로를 탐색해 복도를 연결했습니다. 이를 통해 다른 방을 무분별하게 관통하는 문제를 해결할 수 있었습니다.

하지만 새로운 이슈가 발생했습니다. 복도를 일정 두께로 표현하고 벽을 세우는 과정에서 경로 주변 타일과 겹침 현상이 나타났습니다. 이것은 초기 A* 계산에서 *복도의 두께*를 고려하지 않았기 때문입니다.

던전 전용 그리드를 만들었습니다. 복도의 두께까지 일일이 검사하기 위해서는 각 노드 반경을 확인해야 하지만, 이는 연산 비용이 크게 증가합니다. 그래서 타일 단위 노드 대신 5x5 타일 블록을 하나의 노드로 재정의 했습니다.

다음과 같은 효과를 얻을 수 있었습니다.
- 복도의 두께를 고려하지 않아도 던전 표현에 지장이 없음
- 노드 개수가 25배가량 줄어들어 A* 연산 비용 감소


최종모습
---

//최종 모습 사진


미니맵 구현
---

![Recording 2025-08-30 at 20 55 55](https://github.com/user-attachments/assets/7f9b1c37-c952-4372-a793-7797c4da0d39)

기능 : 유저가 입장한 던전의 모습을 한눈에 볼 수 있는 UI
효과 : 복잡한 던전에서 길을 헤메지 않아 쉽게 게임 플레이 가능

### 던전의 모습을 UI로 시각화

#### 단순히 카메라로 멀리 비춰진 던전을 보여주는 것이 아닌 던전의 모습에 맞게 동적으로 텍스쳐를 만들고 싶었습니다. 처음에는 UI전용 LineRenderer를 구현하여 던전의 모습에 맞게 텍스쳐를 그렸습니다. 하지만 너무 단조로웠고 밋밋해서 바닥은 하얀색, 벽면이나 방 안의 빈 공간은 검은색 선으로 칠해보고 싶었습니다.

#### 외곽 라인을 그리기 위해 임의의 벽 타일을 골라 이어지는 다른 타일을 따라 라인을 그렸습니다. 하지만 벽이 만약 여러개라면 더이상 방문한 벽이 없어질 때 까지 방의 타일들을 여러번 순회해야합니다. 최악의 경우 N^2의 시간복잡도를 가지게 되는데 다행히 외곽라인 알고리즘 관련 자료를 찾게 되어 4N의 시간복잡도로 그칠 수 있었습니다.
https://www.youtube.com/watch?v=ku_thRxLXPw

#### 타일마다 시계방향의 위치 벡터 4개를 저장하는데 순회 중 시작과 끝점이 같거나 서로 반대인 벡터가 있다면 그 벡터를 제거합니다. 그러면 최종적으로 외벽을 이루는 시계방향 벡터, 내벽을 이루는 반시계방향 벡터로 나뉘게 됩니다. 이를 응용해서 내벽을 그릴 때 외벽과 다른 색으로 하는것도 가능합니다.

던전 내비게이션 시스템
---
![Recording 2025-08-30 at 21 09 45](https://github.com/user-attachments/assets/31f5c93a-6540-4d0c-924b-521f18aa3c59)

#### 몬스터는 플레이어를 공격하기 위해, 동전은 플레이어에게 먹히기 위해 자동으로 추적해야 합니다. 단순한 길찾기이기 때문에 익숙한 A star 알고리즘을 사용했으나, 던전에 8마리의 몬스터가 동시에 길찾기를 사용했을 경우 한 프레임에 cpu 전체 사용량의 평균 49%나 차지한다는 점에서 적지 않은 충격을 먹었습니다. 프레임 드랍의 원인이 될 수 있기 때문에 반드시 개선이 필요하다고 생각해 두가지 방법을 고안했습니다.

|  | A star | Unity NavMesh |
| :-:  | :-: | :-: |
| Call| 8 | 8 |
| 평균 CPU 점유율 | 49% | 0.8% |
| GC Allock | 137KB | 0B |
| Time ms | 1.34 | 0.01 |

#### 처음에는 길찾기 요청을 Queue에 담아 프레임마다 순차적으로 처리하는 방식을 구상했습니다. 이 방법은 모든 몬스터의 경로 계산을 여러 프레임으로 분산시킬 수 있어 효율적이라 판단했습니다. 그러나 최악의 경우 특정 프레임에서 병목 현상이 발생할 가능성이 있었습니다. 이를 보완하기 위해 Thread를 활용한 병렬 계산을 고려했지만, Unity WebGL 환경에서는 MultiThread가 지원되지 않는 제약이 있었습니다. 최종적으로는 UniTask를 활용하여 경로 계산 비용을 여러 프레임으로 분산 처리함으로써, 성능을 유지하면서도 WebGL 환경에서도 안정적으로 동작하도록 구현했습니다.

#### 초기 구현에서는 Grid의 Cell을 직접 참조한 뒤 방문한 Cell을 초기화하는 방식으로 경로 계산을 처리했습니다. 그러나 UniTask 기반 비동기 구조로 전환하면서 여러 작업에서 동시에 Cell을 참조하게 되었고, 이로 인해 계산 결과가 불안정해지는 문제가 발생했습니다. 문제를 해결하기 위해 깊은 복사 방식을 도입했지만, 매번 복사가 일어나면서 많은 Garbage가 생성되었습니다. 이에 class 대신 struct를 활용하여 불필요한 메모리 할당을 줄이고, 안정적인 계산과 성능 최적화를 동시에 달성할 수 있었습니다. 이를 통해 GC 발생 빈도를 크게 줄이고, Spike로 인한 프레임 드랍 가능성을 제거했습니다.

![Recording 2025-08-30 at 16 33 59](https://github.com/user-attachments/assets/15795f38-583d-492d-b5ea-bc0ff6b2d8b8)

##### 몬스터가 경로를 역방향으로 이동하는 문제가 발생했습니다. 이는 경로 요청 시점의 몬스터 위치와 실제 경로 계산 완료 시점의 위치가 불일치하여, 몬스터가 과거 위치를 기준으로 경로를 따르면서 나타난 현상이었습니다. 해결 과정에서는 웨이포인트 중 현재 진행 방향과 반대에 위치한 지점을 무시하도록 로직을 수정하였습니다. 이를 위해 몬스터의 실제 이동 벡터와 다음 웨이포인트 방향 벡터 간의 내적 연산을 적용하여, 진행 방향과 일치하지 않는 경우 해당 웨이포인트를 제외하도록 구현하였습니다. 그 결과, 몬스터의 움직임이 자연스러워지고 불필요한 방향 전환으로 인한 이질감이 해소되었습니다.

//성과 표 작성

[^1]: https://www.youtube.com/watch?v=rBY2Dzej03A <br>
https://en.wikipedia.org/wiki/Delaunay_triangulation <br>
https://juliageometry.github.io/DelaunayTriangulation.jl/stable/tutorials/operations_flip_edge/

[^2]: https://en.wikipedia.org/wiki/Curve_orientation

[^3]: http://totologic.blogspot.com/2014/01/accurate-point-in-triangle-test.html

[^4]: https://math.stackexchange.com/questions/3965500/understand-the-relation-between-orientation-in-circle-test-and-determinant

[^5]: https://stackoverflow.com/questions/39984709/how-can-i-check-wether-a-point-is-inside-the-circumcircle-of-3-points<br>
https://stackoverflow.com/questions/2122305/convex-hull-of-4-points
