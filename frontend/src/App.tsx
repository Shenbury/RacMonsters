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

  // Fetch characters from backend
  useEffect(() => {
    const load = async () => {
      try {
        const res = await fetch('/api/characters')
        const data = await res.json()
        if (Array.isArray(data) && data.length >= 2) {
          setPlayer(data[0])
          setEnemy(data[1])
          setPlayerHP(data[0].health)
          setEnemyHP(data[1].health)
          pushLog('Battleants ready')
        }
      } catch (e) {
        pushLog('Failed to load characters')
      }
    }
    load()
  }, [])

  const postAttack = async (attackerId: number, defenderId: number, abilityIndex: number | null) => {
    const body = { attackerId: attackerId, defenderId: defenderId, abilityIndex: abilityIndex }
    const res = await fetch('/api/battle/attack', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body) })
    return res.json()
  }

  const postHeal = async (targetId: number) => {
    const body = { targetId }
    const res = await fetch('/api/battle/heal', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body) })
    return res.json()
  }

  const enemyTurn = async () => {
    if (!player || !enemy || (enemyHP ?? 0) <= 0 || (playerHP ?? 0) <= 0) return
    setBusy(true)
    await new Promise(r => setTimeout(r, 500))
    // enemy chooses random ability or basic
    const choice = Math.random() > 0.6 ? 1 : null
    const result = await postAttack(enemy.id, player.id, choice)
    setPlayerHP(result.defenderHp)
    pushLog(result.message)
    setPlayerAnim('hit')
    setTimeout(() => setPlayerAnim(null), 500)
    setBusy(false)
  }

  const playerAttack = async () => {
    if (busy || !player || !enemy) return
    setBusy(true)
    const result = await postAttack(player.id, enemy.id, null)
    setEnemyHP(result.defenderHp)
    pushLog(result.message)
    setEnemyAnim('hit')
    setTimeout(() => setEnemyAnim(null), 500)
    if ((result.defenderHp ?? 0) > 0) await enemyTurn()
    setBusy(false)
  }

  const playerSpecial = async () => {
    if (busy || !player || !enemy) return
    setBusy(true)
    // use ability index 1 if available
    const idx = player.abilities && player.abilities.length > 1 ? 1 : null
    const result = await postAttack(player.id, enemy.id, idx)
    setEnemyHP(result.defenderHp)
    pushLog(result.message)
    setEnemyAnim('big-hit')
    setTimeout(() => setEnemyAnim(null), 700)
    if ((result.defenderHp ?? 0) > 0) await enemyTurn()
    setBusy(false)
  }

  const playerHeal = async () => {
    if (busy || !player) return
    setBusy(true)
    const result = await postHeal(player.id)
    setPlayerHP(result.defenderHp ?? result.currentHp ?? playerHP)
    pushLog(result.message)
    setPlayerAnim('heal')
    setTimeout(() => setPlayerAnim(null), 700)
    // enemy still takes a turn
    await enemyTurn()
    setBusy(false)
  }

  const reset = async () => {
    // reload characters from server
    try {
      const res = await fetch('/api/characters')
      const data = await res.json()
      if (Array.isArray(data) && data.length >= 2) {
        setPlayer(data[0])
        setEnemy(data[1])
        setPlayerHP(data[0].health)
        setEnemyHP(data[1].health)
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
              </div>
            </div>
          </div>

          <div className="controls">
            <div className="actions">
              <button className="btn attack" onClick={playerAttack} disabled={busy || battleOver} aria-label="Attack">
                Attack
              </button>
              <button className="btn special" onClick={playerSpecial} disabled={busy || battleOver} aria-label="Special Attack">
                Special
              </button>
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
