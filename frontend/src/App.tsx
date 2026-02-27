import { useEffect, useRef, useState } from 'react'
import './App.css'

export default function App() {
  const [playerHP, setPlayerHP] = useState<number | null>(100)
  const [enemyHP, setEnemyHP] = useState<number | null>(100)
  const [busy, setBusy] = useState(false)
  const [log, setLog] = useState<string[]>(['Connecting to arena...'])
  const [darkMode, setDarkMode] = useState(false)
  const [playerAnim, setPlayerAnim] = useState<string | null>(null)
  const [enemyAnim, setEnemyAnim] = useState<string | null>(null)
  const mounted = useRef(true)
  const [player, setPlayer] = useState<any | null>(null)
  const [enemy, setEnemy] = useState<any | null>(null)
  const [sessionId, setSessionId] = useState<string | null>(null)

  useEffect(() => {
    mounted.current = true
    return () => {
      mounted.current = false
    }
  }, [])

  useEffect(() => {
    document.documentElement.dataset.theme = darkMode ? 'dark' : 'light'
  }, [darkMode])

  const pushLog = (s: string) => setLog(l => [s, ...l].slice(0, 6))

  // Create a session on load
  useEffect(() => {
    const start = async () => {
      try {
        const res = await fetch('/api/battle/session', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({}) })
        const data = await res.json()
        if (data?.sessionId && Array.isArray(data.characters) && data.characters.length >= 2) {
          setSessionId(data.sessionId)
          setPlayer(data.characters[0])
          setEnemy(data.characters[1])
          setPlayerHP(data.characters[0].health)
          setEnemyHP(data.characters[1].health)
          pushLog('Battle session started')
        } else {
          pushLog('Failed to start session')
        }
      } catch (e) {
        pushLog('Failed to start session')
      }
    }
    start()
  }, [])

  const postAction = async (actorId: number, action: string, targetId: number | null, abilityIndex: number | null) => {
    if (!sessionId) throw new Error('no session')
    const body = { actorId: actorId, action: action, targetId: targetId, abilityIndex: abilityIndex }
    const res = await fetch(`/api/battle/session/${sessionId}/action`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body) })
    return res.json()
  }

  const doPlayerAttack = async (abilityIndex: number | null) => {
    if (busy || !player || !enemy) return
    setBusy(true)
    const result = await postAction(player.id, 'attack', enemy.id, abilityIndex)
    // result: { characters: [...], messages: [...] }
    if (result?.characters) {
      const chars = result.characters
      // assume first is player and second is enemy as before
      const p = chars.find((c: any) => c.id === player.id) ?? chars[0]
      const e = chars.find((c: any) => c.id === enemy.id) ?? chars[1]
      setPlayer(p)
      setEnemy(e)
      setPlayerHP(p.health)
      setEnemyHP(e.health)
    }
    if (result?.events) {
      // add formatted messages from events (player action first, then AI)
      result.events.forEach((ev: any) => {
        let m = ''
        if (ev.action === 'attack') {
          m = ev.abilityName ? `${ev.actorName} used ${ev.abilityName} and dealt ${ev.damage} damage to ${ev.targetName}.` : `${ev.actorName} hits ${ev.targetName} for ${ev.damage} damage.`
        } else if (ev.action === 'heal') {
          m = `${ev.actorName} healed ${ev.targetName} for ${ev.heal} HP.`
        } else {
          m = `${ev.actorName} ${ev.action}`
        }
        pushLog(m)
      })

      // animations: first event is player's action
      const first = result.events[0]
      if (first) {
        setEnemyAnim(first.action === 'attack' && first.abilityName ? 'big-hit' : 'hit')
        setTimeout(() => setEnemyAnim(null), first.action === 'attack' && first.abilityName ? 700 : 500)
      }
      // second event (if any) is AI response
      if (result.events.length > 1) {
        const ai = result.events[1]
        setPlayerAnim(ai.action === 'attack' && ai.abilityName ? 'big-hit' : 'hit')
        setTimeout(() => setPlayerAnim(null), ai.action === 'attack' && ai.abilityName ? 700 : 500)
      }
    }
    setBusy(false)
  }

  const playerHeal = async () => {
    if (busy || !player) return
    setBusy(true)
    const result = await postAction(player.id, 'heal', player.id, null)
    if (result?.characters) {
      const chars = result.characters
      const p = chars.find((c: any) => c.id === player.id) ?? chars[0]
      const e = chars.find((c: any) => c.id === enemy.id) ?? chars[1]
      setPlayer(p)
      setEnemy(e)
      setPlayerHP(p.health)
      setEnemyHP(e.health)
    }
    if (result?.events) {
      result.events.forEach((ev: any) => {
        let m = ''
        if (ev.action === 'attack') {
          m = ev.abilityName ? `${ev.actorName} used ${ev.abilityName} and dealt ${ev.damage} damage to ${ev.targetName}.` : `${ev.actorName} hits ${ev.targetName} for ${ev.damage} damage.`
        } else if (ev.action === 'heal') {
          m = `${ev.actorName} healed ${ev.targetName} for ${ev.heal} HP.`
        } else {
          m = `${ev.actorName} ${ev.action}`
        }
        pushLog(m)
      })
      // show heal animation for player action
      setPlayerAnim('heal')
      setTimeout(() => setPlayerAnim(null), 700)
      if (result.events.length > 1) {
        const ai = result.events[1]
        setPlayerAnim(ai.action === 'attack' && ai.abilityName ? 'big-hit' : 'hit')
        setTimeout(() => setPlayerAnim(null), ai.action === 'attack' && ai.abilityName ? 700 : 500)
      }
    }
    setBusy(false)
  }

  const reset = async () => {
    // create a new session
    try {
      const res = await fetch('/api/battle/session', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({}) })
      const data = await res.json()
      if (data?.sessionId && Array.isArray(data.characters) && data.characters.length >= 2) {
        setSessionId(data.sessionId)
        setPlayer(data.characters[0])
        setEnemy(data.characters[1])
        setPlayerHP(data.characters[0].health)
        setEnemyHP(data.characters[1].health)
        setLog(['A new battle begins!'])
      }
    } catch {
      setPlayerHP(100)
      setEnemyHP(100)
      setLog(['A new battle begins!'])
    }
    setBusy(false)
  }

  const battleOver = (playerHP ?? 0) <= 0 || (enemyHP ?? 0) <= 0

  return (
    <div className={`app-container battle-theme ${darkMode ? 'dark' : ''}`}>
      <header className="app-header">
        <h1 className="app-title">Monster Battle</h1>
        <p className="app-subtitle">Orange & White theme — dark mode available</p>
      </header>

      <main className="main-content">
        <div className="card battle-card" role="region" aria-label="Battle arena">
          <div className="battle-header">
            <div className="toggles">
              <label className="dark-toggle">
                <input
                  type="checkbox"
                  checked={darkMode}
                  onChange={e => setDarkMode(e.target.checked)}
                  aria-label="Toggle dark mode"
                />
                <span>Dark</span>
              </label>
            </div>
          </div>

          <div className="battle-area">
            <div className="combatant player">
              <div className={`sprite ${playerAnim ?? ''}`} aria-hidden="true">
                {player?.imageUrl ? (
                  <img src={player.imageUrl} alt={player.name} width={120} height={120} />
                ) : (
                  <svg width="120" height="120" viewBox="0 0 100 100">
                    <circle cx="50" cy="50" r="44" fill="#fff" />
                    <circle cx="50" cy="40" r="22" fill="#ff9f1c" />
                  </svg>
                )}
              </div>
              <div className="info">
                <div className="name">{player?.name ?? 'Player'}</div>
                <div className="hp">
                  <div className="hp-bar" aria-hidden="true">
                    <div className="hp-fill" style={{ width: `${playerHP}%` }} />
                  </div>
                  <div className="hp-text">{playerHP ?? '--'} / {player?.maxHealth ?? 100}</div>
                </div>
                <div className="stats">
                  <div>ATK: {player?.attack ?? '--'}</div>
                  <div>DEF: {player?.defense ?? '--'}</div>
                  <div>MATK: {player?.techAttack ?? '--'}</div>
                  <div>MDEF: {player?.techDefense ?? '--'}</div>
                </div>
              </div>
            </div>

            <div className="vs">VS</div>

            <div className="combatant enemy">
              <div className={`sprite ${enemyAnim ?? ''}`} aria-hidden="true">
                {enemy?.imageUrl ? (
                  <img src={enemy.imageUrl} alt={enemy.name} width={120} height={120} />
                ) : (
                  <svg width="120" height="120" viewBox="0 0 100 100">
                    <rect x="6" y="6" width="88" height="88" rx="18" fill="#fff" />
                    <circle cx="50" cy="50" r="20" fill="#ff6b00" />
                  </svg>
                )}
              </div>
              <div className="info">
                <div className="name">{enemy?.name ?? 'Foe'}</div>
                <div className="hp">
                  <div className="hp-bar" aria-hidden="true">
                    <div className="hp-fill enemy" style={{ width: `${enemyHP}%` }} />
                  </div>
                  <div className="hp-text">{enemyHP ?? '--'} / {enemy?.maxHealth ?? 100}</div>
                </div>
                <div className="stats">
                  <div>ATK: {enemy?.attack ?? '--'}</div>
                  <div>DEF: {enemy?.defense ?? '--'}</div>
                  <div>MATK: {enemy?.techAttack ?? '--'}</div>
                  <div>MDEF: {enemy?.techDefense ?? '--'}</div>
                </div>
              </div>
            </div>
          </div>

          <div className="controls">
            <div className="actions">
              <button className="btn attack" onClick={() => doPlayerAttack(null)} disabled={busy || battleOver} aria-label="Attack">
                Attack
              </button>
              {player?.abilities && player.abilities.map((a: any, i: number) => (
                <button key={i} className="btn ability" onClick={() => doPlayerAttack(i)} disabled={busy || battleOver} aria-label={`Use ${a.name}`}>
                  {a.name} {a.power ? `(${a.power})` : ''}
                </button>
              ))}
              <button className="btn heal" onClick={playerHeal} disabled={busy || battleOver} aria-label="Heal">
                Heal
              </button>
            </div>
            <div className="controls-right">
              <button className="btn reset" onClick={reset} aria-label="Restart battle">
                Restart
              </button>
            </div>
          </div>

          <div className="battle-log" aria-live="polite">
            {battleOver && (
              <div className={`result ${((enemyHP ?? 0) <= 0) ? 'win' : 'lose'}`}>
                {((enemyHP ?? 0) <= 0) ? 'You win!' : 'You were defeated'}
              </div>
            )}
            <ul>
              {log.map((l, i) => (
                <li key={i}>{l}</li>
              ))}
            </ul>
          </div>
        </div>
      </main>
    </div>
  )
}
