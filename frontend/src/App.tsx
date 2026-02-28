import React, { useEffect, useRef, useState } from 'react'
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

  // Initialize a battle on load: fetch characters and create a battle
  useEffect(() => {
    const start = async () => {
      try {
        const res = await fetch('/api/characters')
        const chars = await res.json()
        if (Array.isArray(chars) && chars.length >= 2) {
          // pick first two (very dumb)
          const p = chars[0]
          const e = chars[1]

          setPlayer(p)
          setEnemy(e)
          setPlayerHP(p.currentHealth ?? p.currentHealth ?? p.currentHealth ?? (p.maxHealth ?? 100))
          setEnemyHP(e.currentHealth ?? (e.maxHealth ?? 100))

          // create a battle record on the server (server will accept full models)
          try {
            await fetch('/api/battle/create', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ characterA: p, characterB: e, rounds: [] }) })
            pushLog('Battle created on server')
          } catch {
            pushLog('Created local battle (server create failed)')
          }
        } else {
          pushLog('Failed to load characters')
        }
      } catch (e) {
        pushLog('Failed to load characters')
      }
    }
    start()
  }, [])

  // Post a round to the backend. The backend will choose AI move and resolve.
  const postRound = async (round: any) => {
    const res = await fetch('/api/round/execute', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(round) })
    return res.json()
  }

  const doPlayerAttack = async (abilityIndex: number | null) => {
    if (busy || !player || !enemy) return
    setBusy(true)

    // pick ability (if null, choose first ability)
    const playerAbility = abilityIndex != null ? player.abilities[abilityIndex] : (player.abilities && player.abilities[0])
    if (!playerAbility) {
      pushLog('No ability available')
      setBusy(false)
      return
    }

    // prepare round: include PlayerA (player + chosen ability) and PlayerB character (enemy); AI ability left null
    const round = {
      playerA: { character: player, ability: playerAbility },
      playerB: { character: enemy, ability: null }
    }

    // store old hp to compute damage/heal
    const oldPlayerHP = playerHP ?? (player.maxHealth ?? 100)
    const oldEnemyHP = enemyHP ?? (enemy.maxHealth ?? 100)

    const result = await postRound(round)

    // result is the executed Round with updated characters
    if (result) {
      const updatedPlayer = result.playerA?.character ?? player
      const updatedEnemy = result.playerB?.character ?? enemy

      const newPlayerHP = updatedPlayer.currentHealth ?? updatedPlayer.currentHealth ?? (updatedPlayer.maxHealth ?? 100)
      const newEnemyHP = updatedEnemy.currentHealth ?? (updatedEnemy.maxHealth ?? 100)

      // compute differences
      const playerDelta = newPlayerHP - oldPlayerHP
      const enemyDelta = oldEnemyHP - newEnemyHP

      setPlayer(updatedPlayer)
      setEnemy(updatedEnemy)
      setPlayerHP(newPlayerHP)
      setEnemyHP(newEnemyHP)

      // messages
      pushLog(`${player.name} used ${playerAbility.name}.`)
      if (enemyDelta > 0) pushLog(`${enemy.name} took ${enemyDelta} damage.`)
      if (enemyDelta <= 0) pushLog(`${enemy.name} took no damage.`)

      // AI action (if present)
      const aiUsed = result.playerB?.ability
      if (aiUsed) {
        pushLog(`${enemy.name} used ${aiUsed.name}.`)
        if (playerDelta > 0) pushLog(`${player.name} healed ${playerDelta} HP.`)
        if (playerDelta < 0) pushLog(`${player.name} took ${-playerDelta} damage.`)
      }

      // animations: quick naive mapping
      setEnemyAnim('hit')
      setTimeout(() => setEnemyAnim(null), 500)
      if (aiUsed) {
        setPlayerAnim('hit')
        setTimeout(() => setPlayerAnim(null), 500)
      }
    }

    setBusy(false)
  }

  const playerHeal = async () => {
    if (busy || !player || !enemy) return
    setBusy(true)

    const healAbility = player.abilities?.find((a: any) => a.isHeal)
    if (!healAbility) {
      pushLog('No heal available')
      setBusy(false)
      return
    }

    const oldPlayerHP = playerHP ?? (player.maxHealth ?? 100)
    const round = {
      playerA: { character: player, ability: healAbility },
      playerB: { character: enemy, ability: null }
    }

    const result = await postRound(round)
    if (result) {
      const updatedPlayer = result.playerA?.character ?? player
      const updatedEnemy = result.playerB?.character ?? enemy
      const newPlayerHP = updatedPlayer.currentHealth ?? (updatedPlayer.maxHealth ?? 100)
      const newEnemyHP = updatedEnemy.currentHealth ?? (updatedEnemy.maxHealth ?? 100)

      setPlayer(updatedPlayer)
      setEnemy(updatedEnemy)
      setPlayerHP(newPlayerHP)
      setEnemyHP(newEnemyHP)

      const playerDelta = newPlayerHP - oldPlayerHP
      pushLog(`${player.name} used ${healAbility.name}.`)
      if (playerDelta > 0) pushLog(`${player.name} healed ${playerDelta} HP.`)

      // AI action
      const aiUsed = result.playerB?.ability
      if (aiUsed) {
        pushLog(`${enemy.name} used ${aiUsed.name}.`)
      }

      setPlayerAnim('heal')
      setTimeout(() => setPlayerAnim(null), 700)
    }
    setBusy(false)
  }

  const reset = async () => {
    try {
      const res = await fetch('/api/characters')
      const chars = await res.json()
      if (Array.isArray(chars) && chars.length >= 2) {
        const p = chars[0]
        const e = chars[1]
        setPlayer(p)
        setEnemy(e)
        setPlayerHP(p.currentHealth ?? (p.maxHealth ?? 100))
        setEnemyHP(e.currentHealth ?? (e.maxHealth ?? 100))
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
