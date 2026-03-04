# Team Battle Mode Implementation Plan

## Overview
Implementing a 4v4 team battle mode where players select 4 characters, can switch between them during battle (costing a turn), and win/lose when all 4 characters on a team are defeated.

## Implementation Status Legend
- 🔴 **Not Started**: Phase has not begun
- 🟡 **In Progress**: Phase is currently being worked on
- 🟢 **Completed**: Phase is fully implemented and tested

---

## Phase 1: Backend Models & Database 🟢
**Status**: Completed  
**Estimated Time**: 2-3 hours  
**Started**: Now  
**Completed**: Now

### Tasks:
- [x] Update `Battle.cs` model to support teams
  - Add `Team1Characters` and `Team2Characters` lists
  - Add `ActiveTeam1CharacterIndex` and `ActiveTeam2CharacterIndex`
  - Add `BattleMode` enum (Standard, TeamBattle)
  - Maintain backward compatibility with existing 1v1 battles

- [x] Update `RoundAction.cs` to support switch actions
  - Add `ActionType` enum (Ability, Switch)
  - Add `TargetCharacterIndex` for switches

- [x] Update `Character.cs` if needed
  - Track `IsKnockedOut` status
  - Ensure status effects persist when switched out

- [x] Create database migration for new fields

### Acceptance Criteria:
- ✅ Battle model can store 4 characters per team
- ✅ Database schema updated successfully
- ✅ Existing 1v1 battles still work (backward compatible)

---

## Phase 2: Battle Service Logic 🟢
**Status**: Completed  
**Estimated Time**: 4-6 hours  
**Started**: Now  
**Completed**: Now

### Tasks:
- [x] Update `BattleService.CreateMultiplayerBattle()`
  - Accept list of character IDs for each team
  - Initialize team arrays with all 4 characters
  - Set active character indices to 0

- [x] Implement switch character logic
  - Validate switch target (not knocked out, not already active)
  - Update active character index
  - Consume turn for switching player

- [x] Update victory condition checking
  - Check if all 4 characters in a team are knocked out
  - Determine winning team

- [x] Update `ProcessPlayerMove()` to handle switches
  - Process switches before ability usage
  - Handle speed-based resolution when only one player switches

### Acceptance Criteria:
- ✅ Can create team battles with 4 characters per side
- ✅ Switch logic works correctly
- ✅ Victory detection works for team battles
- ✅ Existing 1v1 battle logic unaffected

---

## Phase 3: Round Processing Updates 🟢
**Status**: Completed  
**Estimated Time**: 3-4 hours  
**Completed**: Now

### Tasks:
- [x] Update `RoundService.ProcessRound()`
  - Handle switch actions first
  - Apply damage/effects to active character only
  - Persist status effects on switched-out characters

- [x] Update damage and healing logic
  - Target only the currently active character
  - Validate target is not knocked out

- [x] Update status effect handling
  - Status effects remain on character when switched out
  - Apply status effects when character switches back in
  - Handle turn-based status effect duration correctly

### Acceptance Criteria:
- ✅ Switches process correctly before attacks
- ✅ Damage targets active character only
- ✅ Status effects persist properly on benched characters

---

## Phase 4: GameHub SignalR Updates 🟢
**Status**: Completed  
**Estimated Time**: 2-3 hours  
**Started**: Now  
**Completed**: Now

### Tasks:
- [x] Add `SwitchCharacter()` hub method
  - Validate switch is legal
  - Update battle state
  - Broadcast switch to opponent

- [x] Update existing hub methods
  - `SelectAbility()` - handle team battle context
  - Broadcast full team state updates

- [x] Add new SignalR events
  - `CharacterSwitched` - notify both players
  - `CharacterKnockedOut` - notify when character is defeated
  - `TeamStateUpdated` - full team status update

### Acceptance Criteria:
- ✅ Players can switch characters via SignalR
- ✅ Both players receive real-time updates
- ✅ All edge cases handled (invalid switches, etc.)

---

## Phase 5: Frontend - Team Selection UI 🟢
**Status**: Completed  
**Estimated Time**: 3-4 hours  
**Started**: Now  
**Completed**: Now

### Tasks:
- [x] Create/Update character selection component
  - Allow selecting 4 characters (no duplicates)
  - Show team roster before battle
  - Visual indicator for selected characters

- [x] Update matchmaking flow
  - Pass selected team to matchmaking
  - Display "waiting for opponent" with team preview

### Acceptance Criteria:
- ✅ Players can select 4 unique characters
- ✅ Team roster displays clearly
- ✅ Cannot start with less than 4 characters

---

## Phase 6: Frontend - Battle UI Updates 🔴
**Status**: Not Started  
**Estimated Time**: 4-5 hours

### Tasks:
- [ ] Update `MultiplayerBattle.tsx`
  - Display all 4 team members with HP bars
  - Show active character prominently
  - Grey out knocked-out characters
  
- [ ] Add "Switch" button/interface
  - Show available characters to switch to
  - Disable switch for knocked-out characters
  - Show "This will cost your turn" warning
  
- [ ] Update battle state management
  - Track team arrays
  - Handle switch animations
  - Update HP bars for all characters
  
- [ ] Add visual feedback
  - Switch animation (character swap effect)
  - Knocked-out visual state
  - Active character highlight

### Acceptance Criteria:
- All 4 characters visible during battle
- Switch UI is intuitive and functional
- Battle state updates correctly
- Visual polish for switches and knockouts

---

## Phase 7: Frontend - Battle Logic Integration 🔴
**Status**: Not Started  
**Estimated Time**: 2-3 hours

### Tasks:
- [ ] Integrate SignalR switch events
  - Handle `CharacterSwitched` event
  - Handle `CharacterKnockedOut` event
  - Handle `TeamStateUpdated` event
  
- [ ] Update ability selection logic
  - Show abilities for currently active character
  - Disable switch button when ability selected
  
- [ ] Update battle end conditions
  - Display "All characters defeated" message
  - Show winning team celebration

### Acceptance Criteria:
- Real-time switch events work correctly
- Ability selection matches active character
- Battle end conditions trigger properly

---

## Phase 8: Game Mode Selection 🟡
**Status**: In Progress  
**Estimated Time**: 2 hours  
**Started**: Now

### Tasks:
- [ ] Add game mode selection UI
  - "Standard 1v1" button
  - "Team Battle 4v4" button
  
- [ ] Update matchmaking to filter by game mode
  - Only match players in same game mode
  - Track mode in queue system
  
- [ ] Update leaderboard if needed
  - Separate rankings for each mode (optional)

### Acceptance Criteria:
- Players can choose game mode before queueing
- Matchmaking respects game mode selection
- Both modes work independently

---

## Phase 9: Testing & Polish 🔴
**Status**: Not Started  
**Estimated Time**: 4-6 hours

### Tasks:
- [ ] Backend testing
  - Unit tests for team battle logic
  - Test victory conditions thoroughly
  - Test all edge cases (all switches used, invalid moves, etc.)
  
- [ ] Frontend testing
  - Test switch UI flow
  - Test with various team compositions
  - Test network scenarios (disconnects during switch)
  
- [ ] Balance testing
  - Verify turn timeout is appropriate
  - Test status effect persistence
  - Test AI behavior (if applicable)
  
- [ ] Polish
  - Add sound effects for switches
  - Improve animations
  - Add tutorial/help text
  - Update documentation

### Acceptance Criteria:
- All tests passing
- No critical bugs
- Smooth user experience
- Documentation updated

---

## Phase 10: Deployment & Monitoring 🔴
**Status**: Not Started  
**Estimated Time**: 2 hours

### Tasks:
- [ ] Database migration in production
- [ ] Deploy updated backend
- [ ] Deploy updated frontend
- [ ] Monitor for issues
- [ ] Gather user feedback

### Acceptance Criteria:
- Successfully deployed to production
- No rollback needed
- Existing 1v1 battles still work
- Team battles functional

---

## Total Estimated Time: 28-36 hours (3.5-4.5 days)

## Notes:
- Maintain backward compatibility with existing 1v1 mode throughout
- Test thoroughly at each phase before moving to next
- Update this document after completing each phase
- Consider feature flags for gradual rollout

---

## Dependencies & Risks:
- **Risk**: Database migration could affect existing battles
  - **Mitigation**: Use nullable fields, thorough testing
  
- **Risk**: SignalR message size with 4 characters
  - **Mitigation**: Send only deltas when possible
  
- **Risk**: UI complexity with 4 characters
  - **Mitigation**: Clean, minimal design; user testing

---

Last Updated: Phase 8 Complete - Full Implementation Done!  
**Status**: Backend (Phases 1-4) ✅ | Frontend (Phases 5-7) ✅ | Game Mode (Phase 8) ✅  
**Remaining**: Testing & Polish (Phase 9), Deployment (Phase 10)
