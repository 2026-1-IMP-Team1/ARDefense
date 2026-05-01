// It seems we can manage the game flow by dividing it into 6 states.
// GAME_START: When a game starts for the very first time
// BEFORE_GATE_OPENED: The preparation phase before the gate opens
// NORMAL_MONSTER_SPAWN: The phase where normal monsters spawn
// ELITE_MONSTER_SPAWN: The phase where elite monsters spawn
// BOSS_MONSTER_SPAWN: The phase where the boss monster spawns
// GAME_OVER: Game over (when retiring mid-game or clearing the game)
// Example game flow: G_S - B_G_O - N_M_S - E_M_S - B_M_S - B_G_O - N_M_S - ... - E_M_S - G_O (retire during elite monster phase)

// Since the number of times elite monsters are spawned is not that high,
// it might be good to integrate and manage the ELITE_MONSTER_SPAWN phase with the NORMAL_MONSTER_SPAWN phase.
// Feel free to share your opinions and modify the code as you wish!

public enum GameFlowState
{
    GAME_START,          // G_S
    BEFORE_GATE_OPEN,    // B_G_O
    NORMAL_MONSTER_SPAWN, // N_M_S
    ELITE_MONSTER_SPAWN,  // E_M_S
    BOSS_MONSTER_SPAWN,   // B_M_S
    GAME_OVER,           // G_O
    GAME_CLEAR           // G_C  (Final boss defeated)
}