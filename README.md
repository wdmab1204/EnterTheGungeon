## About me
- 게임 개발에서 가장 중요한 협업 능력을 위해 고민하고 개선합니다.
- 개발 중 아이디어가 떠오르면 팀원들과 공유하여 게임 퀄리티 향상에 기여합니다.
- 코드는 문서라고 생각하여, 팀원들이 쉽게 읽을 수 있도록 가독성을 중요시 합니다.
- 원할한 게임 플레이를 위해 성능 개선을 끊임없이 고민하고 생각합니다.

## 보유 기술
#### 1. C#, Unity Engine
  - C#과 Unity Engine을 이용하여 "Enter The Gungeon"게임을 제작하였고 현업에서도 사용중입니다.
#### 2. Github, Tortoise SVN
  - 학생 때 Github를 사용하여 협업의 기초를 배웠고, 현업에서 Toirtoise SVN을 사용하여 협업 실무를 배웠습니다.
#### 3. Yona
  - 실무에서 Yona를 이용해 프로젝트의 작업 상황, 이슈를 공유하여 일정을 관리했습니다. 
#### 4. 커뮤니케이션 능력
  - 게임잼에 참가하여 팀원들과 협업하여 게임을 만들었고, 실무에서 2년 근무하며 회의, 기획 논의 등으로 일을 진행하였습니다.

## Enter the Gungeon
![image](https://github.com/user-attachments/assets/37c2967b-158c-4c13-91d2-9ed5a920728f)

### 개요
- 게임 장르 : Roguelike
- 작업 기간 : 2024.10.15 ~ 2024.12.23(약 4개월)
- 기술 : C#, Unity Engine

### 프로젝트를 통해 얻은 점
#### CSV Load 최적화
- 문제점
  - 대량의 CSV파일을 읽을 때 속도가 많이 느려 게임을 켜는데 많은 시간이 걸렸습니다.
- 해결점
  - CSV를 Bytes파일로 변환하고 기존 Parsing의 문제점을 보완하여 시간을 단축시켰습니다. 그리고 파일 변환 툴을 별도로 제작하여 프로젝트를 열지 않고 파일을 조작할 수 있습니다.
- [[코드 링크]](https://github.com/wdmab1204/EnterTheGungeon/tree/main/Client/Assets/Scripts/ScriptLoader)
- [[툴 링크]](https://github.com/wdmab1204/EnterTheGungeon/tree/main/Tool)

#### 퀘스트UI 최적화
- 문제점
  - ScrollView를 사용하면 프레임이 급격하게 낮아져 버벅거리는 현상이 있었습니다.
- 해결점
  - 게임오브젝트를 재사용하여 Scroll 연산 비용을 절감했습니다.
- [[코드 링크]](https://github.com/wdmab1204/EnterTheGungeon/tree/main/Client/Assets/Scripts/UI)

#### 컴포넌트를 활용하여 중복 코드를 제거하고 재사용 했습니다.
- 문제점
  - 몬스터 패턴을 구현할 때 마다 똑같은 기능을 여러번 만들어 중복 코드가 많아지고 유지보수에 어려움을 느꼈습니다.
- 해결점
  - 기능을 컴포넌트로 묶어 중복되는 코드를 줄였고 인스펙터로 세밀한 패턴 조작을 할 수 있도록 구조를 개선했습니다.
- [[코드 링크]](https://github.com/wdmab1204/EnterTheGungeon/blob/main/Client/Assets/Scripts/Bullet/FanShapeShooting.cs)

### 주요 내용
#### 1. byte배열 역직렬화 로직 구현
  - CSV파일을 읽을 때 수 초 이상의 시간이 걸려 게임을 켜는데 오랜 시간이 걸렸습니다.
  - 리틀 엔디안 방식으로 파일 변환 툴에서 CSV내용을 byte형태의 데이터로 변환했습니다.
  - 기존 CSV Parsing로직에서 리플렉션 사용을 제거하는 대신 자식클래스에서 역직렬화 로직을 직접 구현하는 방법으로 효율성을 극대화 했습니다.
 
#### 2. 랜덤 던전 생성기 구현
  - 이진트리를 이용하여 2의 n승 개수만큼 방이 있는 던전을 자동으로 생성하는 기능을 구현했습니다.
  - 부모 노드와 자식 노드의 방 위치를 계산하여 두 곳을 잇는 다리를 자동으로 생성하는 기능을 구현했습니다.
  - 방이 생성되지 않은 외부 공간은 콜라이더를 생성하여 플레이어 및 몬스터가 방 바깥으로 나가지 못하도록 구현했습니다.
