# TheWayHome
엄마고양이를 찾아 떠나는 2D 플랫포머 장르의 게임

### 23-06-07
1. 계정 리셋 버튼 구현
2. 이어하기 시에도 캐릭터 선택할 수 있도록 함
3. 게임 강제종료 시에도 최대스테이지를 판단해서 스테이지 정보 저장할 수 있게 함
4. 통로 기어가기 중 일어날 수 있는 문제 수정

### 23-06-06
1. 로비, 인게임에서 슬라이더를 통한 배경음, 효과음 볼륨 조절 구현
2. 스테이지 정보를 json으로 관리할 싱글톤 데이터 매니저 생성
3. 이어하기 구현

### 23-06-05
1. 로비, 인게임 UI, 버튼 작업 중
2. 사운드 매니저 제작
3. 사운드 탐색 및 적용

### 23-06-04
1. 로비 씬 제작
2. 로비에서 캐릭터 선택 후 게임 시작시 선택한 캐릭터로 플레이 가능
3. 로비, 인게임 UI 및 버튼 배치 및 기능 구현 중

### 23-06-03
1. 튜토리얼 마무리
2. 엄폐물 기능 구현(엄폐물 안에 있을 시 적의 추격 해제, 적이 통과)
3. 엔드포인트 도달 시 다음 스테이지 씬으로 이동
4. 공격 시엔 이동 방지

### 23-06-02
1. 튜토리얼 제작 중
2. 플레이어 피격, 사망 구현
3. 적 프리팹 생성
4. 적 인공지능 구현(기본 상태: 느린 속도로 랜덤방향 이동, 플레이어 발견: 빠른 속도로 플레이어에게 이동)
5. 플레이어 넉백 구현

### 23-06-01
1. 튜토리얼 제작 중
2. 박스 파괴시 50% 확률로 생선 드롭
3. 생선 먹을 시 체력 회복
4. 시간에 따라 감소하는 체력바 구현(플레이어 체력: 100, 초당 감소: 1)

### 23-05-31
1. 튜토리얼 스테이지 제작 중
2. 조작가이드를 TextMesh로 각 조작이 필요한 위치에 배치
3. 플레이어가 지나가면 떨어지는 고드름 생성
4. 엄폐물 프리팹 생성(기능은 아직)
5. 플레이어 공격 구현
6. 현실 시간 기준으로 배경 밤낮이 바뀌도록 설정

### 23-05-30
1. 애니메이션 버그 수정
2. 타일 팔레트 생성
3. 튜토리얼 스테이지 제작 (1/2)

### 23-05-28
1. 벽타고 미끄러지기 구현
2. 벽타고 오르기 구현
3. 애니메이터 작업
4. 코드 정리
5. UI 탐색

### 23-05-27
1. 엎드리기, 기어가기 구현 및 애니메이션 작업
2. 대쉬 구현 및 애니메이션 작업

### 23-05-26
1. 플레이어 이동, 점프 구현
2. 이동, 점프, 체공, 착륙 애니메이션 구현

### 23-05-25
1. 프로젝트 생성, 에셋 임포트
2. 깃 저장소 연동
3. 캐릭터 5종 애니메이션 생성 및 애니메이터 오버라이드 컨트롤러 생성