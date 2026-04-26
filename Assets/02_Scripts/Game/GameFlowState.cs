// Game의 Flow를 총 6가지로 나누어 관리할 수 있을 거 같습니다.
// GAME_START: 한 게임이 완전 처음 시작할 때
// BEFORE_GATE_OPENED: Gate가 열리기 전 정비하는 페이즈
// NORMAL_MONSTER_SPAWN: 일반 몬스터가 스폰되는 페이즈
// ELITE_MONSTER_SPAWN: 엘리트 몬스터가 스폰되는 페이즈
// BOSS_MONSTER_SPAWN: 보스 몬스터가 스폰되는 페이즈
// GAME_OVER: 게임 종료 (중간에 리타이어하거나, 게임을 클리어할 시)
// 예시 게임 플로우: G_S - B_G_O - N_M_S - E_M_S - B_M_S - B_G_O - N_M_S - ... - E_M_S - G_O (엘리트 몬스터 페이즈에서 리타이어)

// 엘리트 몬스터를 스폰하는 횟수는 그렇게 많지 않으므로,
// ELITE_MONSTER_SPAWN 페이즈를 NORMAL_MONSTER_SPAWN 페이즈와 통합하여 관리하여도 좋을 거 같습니다.
// 자유롭게 의견 내주시고 마음껏 코드 수정해주세요!

public enum GameFlowState
{
    GAME_START,          // G_S
    BEFORE_GATE_OPEN,    // B_G_O
    NORMAL_MONSTER_SPAWN, // N_M_S
    ELITE_MONSTER_SPAWN,  // E_M_S
    BOSS_MONSTER_SPAWN,   // B_M_S
    GAME_OVER,           // G_O
    GAME_CLEAR           // G_C  (최종 보스 처치)
}