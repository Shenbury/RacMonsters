import React, { useEffect, useRef, useState } from 'react'
import './App.css'

type GameState = 'select' | 'battle' | 'victory' | 'defeat'

/**
 * Minimal shapes inferred from usage in the component.
 */
interface Ability {
    name?: string
    Name?: string
    power?: number
    Power?: number
    description?: string
    Description?: string
    isTech?: boolean
    IsTech?: boolean
    isHeal?: boolean
    IsHeal?: boolean
}

interface Character {
    id: string | number
    name?: string
    imageUrl?: string
    currentHealth?: number
    maxHealth?: number
    attack?: number
    defense?: number
    techAttack?: number
    techDefense?: number
    abilities?: Ability[]
    // allow other unknown fields the backend may include
    [key: string]: any
}

const App: React.FC = () => {
    const [allChars, setAllChars] = useState<Character[]>([])
    const [playerHP, setPlayerHP] = useState<number | null>(null)
    const [enemyHP, setEnemyHP] = useState<number | null>(null)
    const [busy, setBusy] = useState(false)
    const [log, setLog] = useState<string[]>(['Welcome to the arena'])
    const [darkMode, setDarkMode] = useState(false)
    const [playerAnim, setPlayerAnim] = useState<string | null>(null)
    const [enemyAnim, setEnemyAnim] = useState<string | null>(null)
    const [playerMiss, setPlayerMiss] = useState(false)
    const [enemyMiss, setEnemyMiss] = useState(false)
    const mounted = useRef(true)
    const [player, setPlayer] = useState<Character | null>(null)
    const [enemy, setEnemy] = useState<Character | null>(null)
    const [opponents, setOpponents] = useState<Character[]>([])
    const [gameState, setGameState] = useState<GameState>('select')
    const [hoveredChar, setHoveredChar] = useState<Character | null>(null)

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

    const load = async () => {
        try {
            const res = await fetch('/api/characters')
            const chars = await res.json()
            if (Array.isArray(chars) && chars.length > 0) {
                setAllChars(chars)
                opponents && opponents.length === 0 && setOpponents(chars) // set opponents if not already set
            }
        } catch {
            pushLog('Failed to load characters')
        }
    }

    // load characters for selection
    useEffect(() => {
        load()
    }, [])

    // Post a round to the backend. The backend will choose AI move and resolve.
    const postRound = async (round: any) => {
        const res = await fetch('/api/round/execute', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(round) })
        return res.json()
    }

    const pickRandomOpponent = (available: Character[] | null | undefined): Character | null => {
        if (!available || available.length === 0) return null
        const idx = Math.floor(Math.random() * available.length)
        return available[idx]
    }

    const startWithPlayer = (char: Character) => {
        // choose player; opponents are others
        const p = { ...char }
        const others = allChars.filter(c => c.id !== char.id).map(c => ({ ...c }))
        setPlayer(p)
        setOpponents(others)
        setPlayerHP(p.currentHealth ?? p.maxHealth ?? 100)

        // pick random opponent
        const opp = pickRandomOpponent(others)
        if (opp) {
            setEnemy(opp)
            setEnemyHP(opp.currentHealth ?? opp.maxHealth ?? 100)
            setGameState('battle')
            pushLog(`${p.name} enters the arena!`)
            pushLog(`${opp.name} appears as your foe.`)
        } else {
            setGameState('victory')
        }
    }

    // (Previously unused) helper removed to satisfy strict linting rules

    const handleEnemyDefeated = (defeatedEnemy: any) => {
        // update opponents and pick next deterministically from the updated list (avoid stale state)
        setOpponents((prev: Character[]) => {
            const updated = prev.map(o => o.id === defeatedEnemy.id ? { ...o, currentHealth: 0 } : o)
            const alive = updated.filter(o => (o.currentHealth ?? o.maxHealth ?? 0) > 0)
            if (alive.length === 0) {
                setGameState('victory')
                pushLog('You have beaten the game!')
            } else {
                const next = pickRandomOpponent(alive)
                if (next) {
                    setEnemy(next)
                    setEnemyHP(next.currentHealth ?? next.maxHealth ?? 100)
                    // reset player health for the next fight
                    setPlayer((prevPlayer: any | null) => {
                        if (!prevPlayer) return prevPlayer
                        const resetPlayer = { ...prevPlayer, currentHealth: prevPlayer.maxHealth }
                        return resetPlayer
                    })
                    setPlayerHP(player?.maxHealth ?? (playerHP ?? 100))
                    pushLog(`${defeatedEnemy.name} was defeated. ${next.name} approaches!`)
                }
            }
            return updated
        })
    }

    const doPlayerAttack = async (abilityIndex: number | null) => {
        if (busy || !player || !enemy || gameState !== 'battle') return
        setBusy(true)

        const playerAbility = abilityIndex != null ? player.abilities?.[abilityIndex] : (player.abilities && player.abilities[0])
        if (!playerAbility) {
            pushLog('No ability available')
            setBusy(false)
            return
        }

        const round = {
            playerA: { character: player, ability: playerAbility },
            playerB: { character: enemy, ability: null }
        }

        const oldEnemyHP = enemyHP ?? (enemy.maxHealth ?? 100)

        const result = await postRound(round)

        if (result) {
            const updatedPlayer = result.playerA?.character ?? player
            const updatedEnemy = result.playerB?.character ?? enemy

            const newPlayerHP = updatedPlayer.currentHealth ?? (updatedPlayer.maxHealth ?? 100)
            const newEnemyHP = updatedEnemy.currentHealth ?? (updatedEnemy.maxHealth ?? 100)

            // compute differences
            const enemyDelta = oldEnemyHP - newEnemyHP

            // update local copies
            setPlayer(updatedPlayer)
            setPlayerHP(newPlayerHP)
            setEnemy(updatedEnemy)
            setEnemyHP(newEnemyHP)

            const playerAbilityName = playerAbility.name ?? playerAbility.Name ?? 'Ability'
            pushLog(`${player.name} used ${playerAbilityName}.`)
            if (enemyDelta > 0) pushLog(`${enemy.name} took ${enemyDelta} damage.`)
            if (enemyDelta <= 0) pushLog(`${enemy.name} took no damage.`)

            // Structured results from server actions (prefer numeric fields)
            const pAction = result.playerA
            const eAction = result.playerB

            const pHit = pAction ? (pAction.hit ?? pAction.Hit) : undefined
            const pDamage = pAction ? (pAction.damage ?? pAction.Damage) : undefined
            const pHeal = pAction ? (pAction.healAmount ?? pAction.HealAmount) : undefined

            if (pHit === false) pushLog(`${player.name} missed!`)
            else if (pHeal) pushLog(`${player.name} healed ${pHeal} HP.`)
            else if (pDamage) pushLog(`${player.name} dealt ${pDamage} damage to ${enemy.name}.`)

            const aiUsed = eAction?.ability ?? result.playerB?.ability
            if (aiUsed) {
                const aiAbilityName = aiUsed.name ?? aiUsed.Name ?? 'Ability'
                pushLog(`${enemy.name} used ${aiAbilityName}.`)
            }

            const eHit : boolean | undefined = eAction ? (eAction.hit ?? eAction.Hit) : undefined
            const eDamage : number | undefined = eAction ? (eAction.damage ?? eAction.Damage) : undefined
            const eHeal : number | undefined = eAction ? (eAction.healAmount ?? eAction.HealAmount) : undefined

            if (eHit === false) pushLog(`${enemy.name} missed!`)
            else if (eHeal) pushLog(`${enemy.name} healed ${eHeal} HP.`)
            else if (eDamage) pushLog(`${enemy.name} dealt ${eDamage} damage to ${player.name}.`)

            // animations based on structured fields
            const clearPlayerAnim = (ms = 700) => setTimeout(() => setPlayerAnim(null), ms)
            const clearEnemyAnim = (ms = 700) => setTimeout(() => setEnemyAnim(null), ms)

            // Player action effects
            if (pHit === false) {
                setPlayerMiss(true)
                setTimeout(() => setPlayerMiss(false), 1000)
            } else if (pHeal) {
                setPlayerAnim('glow-green')
                clearPlayerAnim(1000)
            } else if (pDamage) {
                setPlayerAnim('shake')
                setEnemyAnim('glow-red shudder')
                clearPlayerAnim(600)
                clearEnemyAnim(600)
            }

            // Enemy action effects
            if (eHit === false) {
                setEnemyMiss(true)
                setTimeout(() => setEnemyMiss(false), 1000)
            } else if (eHeal) {
                setEnemyAnim('glow-green')
                clearEnemyAnim(1000)
            } else if (eDamage) {
                setEnemyAnim('shake')
                setPlayerAnim(prev => (prev ? prev + ' glow-red shudder' : 'glow-red shudder'))
                clearEnemyAnim(600)
                clearPlayerAnim(600)
            }

            // check for defeat/victory
            if ((newEnemyHP ?? 0) <= 0) {
                pushLog(`${enemy.name} has been defeated.`)
                handleEnemyDefeated(enemy)
            }

            if ((newPlayerHP ?? 0) <= 0) {
                setGameState('defeat')
                pushLog(`${player.name} has fallen...`)
            }
        }

        setBusy(false)
    }

    // Player heal handler removed; healing is handled through abilities posted to the server

    const resetAll = () => {
        setPlayer(null)
        setEnemy(null)
        setOpponents([])
        setPlayerHP(null)
        setEnemyHP(null)
        setGameState('select')
        setLog(['Choose your champion'])
    }

    const playerMax = player?.maxHealth ?? 100
    const playerHPVal = playerHP ?? playerMax
    const playerHPPercent = Math.max(0, Math.min(100, Math.round((playerHPVal / playerMax) * 100)))

    const enemyMax = enemy?.maxHealth ?? 100
    const enemyHPVal = enemyHP ?? enemyMax
    const enemyHPPercent = Math.max(0, Math.min(100, Math.round((enemyHPVal / enemyMax) * 100)))

    return (
        <div className={`app-container battle-theme ${darkMode ? 'dark' : ''}`}>
            <header className="app-header">
                <h1 className="app-title">RAC Battle</h1>
            </header>

            <main className="main-content">
                {gameState === 'select' && (
                    <div className="card select-card">
                        <h2 className="section-title">Choose your champion</h2>
                        <div className="char-grid">
                            {allChars.map((c) => (
                                <div key={c.id} className="char-tile-wrapper">
                                    <button
                                        className="char-tile"
                                        onClick={() => startWithPlayer(c)}
                                        onMouseEnter={() => setHoveredChar(c)}
                                        onMouseLeave={() => setHoveredChar(null)}
                                        onFocus={() => setHoveredChar(c)}
                                        onBlur={() => setHoveredChar(null)}
                                    >
                                        <div className="char-sprite">
                                            {c.imageUrl ? <img src={c.imageUrl} alt={c.name} /> : (
                                                <svg width="64" height="64" viewBox="0 0 100 100"><circle cx="50" cy="50" r="44" fill="#fff" /><circle cx="50" cy="40" r="22" fill="#ff9f1c" /></svg>
                                            )}
                                        </div>
                                        <div className="char-name">{c.name}</div>
                                        <div className="char-stats">HP: {c.maxHealth ?? '--'} | ATK: {c.attack ?? '--'} | DEF: {c.defense ?? '--'}</div>
                                        <div className="char-stats">TATK: {c.techAttack ?? '--'} | TDEF: {c.techDefense ?? '--'}</div>
                                    </button>

                                    {hoveredChar?.id === c.id && c.abilities && c.abilities.length > 0 && (
                                        <div className="hover-abilities" aria-hidden="true">
                                            <div className="hover-abilities-title">Abilities</div>
                                            <ul>
                                                {c.abilities.map((ab: Ability, idx: number) => {
                                                    const name = ab.name ?? ab.Name ?? 'Ability'
                                                    return (
                                                        <li key={idx} className="hover-ability-item">
                                                            <span className="hover-ability-name">{name}</span>
                                                        </li>
                                                    )
                                                })}
                                            </ul>
                                        </div>
                                    )}
                                </div>
                            ))}
                        </div>
                    </div>
                )}

                {gameState === 'battle' && (
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
                                    {playerMiss && (
                                        <div className="miss-badge" aria-hidden="true">?</div>
                                    )}
                                </div>
                                <div className="info">
                                    <div className="name">{player?.name ?? 'Player'}</div>
                                    <div className="hp">
                                        <div className="hp-bar" aria-hidden="true">
                                            <div className="hp-fill" style={{ width: `${playerHPPercent}%` }} />
                                        </div>
                                        <div className="hp-text">{playerHPVal ?? '--'} / {playerMax}</div>
                                    </div>
                                    <div className="stats">
                                        <div>ATK: {player?.attack ?? '--'}</div>
                                        <div>DEF: {player?.defense ?? '--'}</div>
                                        <div>TATK: {player?.techAttack ?? '--'}</div>
                                        <div>TDEF: {player?.techDefense ?? '--'}</div>
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
                                    {enemyMiss && (
                                        <div className="miss-badge" aria-hidden="true">?</div>
                                    )}
                                </div>
                                <div className="info">
                                    <div className="name">{enemy?.name ?? 'Foe'}</div>
                                    <div className="hp">
                                        <div className="hp-bar" aria-hidden="true">
                                            <div className="hp-fill enemy" style={{ width: `${enemyHPPercent}%` }} />
                                        </div>
                                        <div className="hp-text">{enemyHPVal ?? '--'} / {enemyMax}</div>
                                    </div>
                                    <div className="stats">
                                        <div>ATK: {enemy?.attack ?? '--'}</div>
                                        <div>DEF: {enemy?.defense ?? '--'}</div>
                                        <div>TATK: {enemy?.techAttack ?? '--'}</div>
                                        <div>TDEF: {enemy?.techDefense ?? '--'}</div>
                                    </div>
                                </div>
                            </div>
                        </div>

            <div className="controls">
                            <div className="actions ability-grid">
                                {player?.abilities && player.abilities.map((a: Ability, i: number) => {
                                    const name = a.name ?? a.Name ?? 'Ability'
                                    const power = a.power ?? a.Power
                                    const description = a.description ?? a.Description ?? ''
                                    const isTech = a.isTech ?? a.IsTech
                                    const isHeal = a.isHeal ?? a.IsHeal
                                    return (
                                        <div key={i} className="ability-block">
                                            <button className="btn ability" onClick={() => doPlayerAttack(i)} disabled={busy} aria-label={`Use ${name}`}>
                                                {name} {power ? `(${power})` : ''}
                                            </button>
                                            <div className="ability-meta">
                                                {description && <div className="ability-desc">{description}</div>}
                                                <div className="ability-tags">{isTech ? 'Tech' : isHeal ? 'Heal' : ''}{(isTech || isHeal) && power ? ` • Power: ${power}` : (!isTech && !isHeal && power ? `Power: ${power}` : '')}</div>
                                            </div>
                                        </div>
                                    )
                                })}
                            </div>
                            <div className="controls-right">
                                <button className="btn reset" onClick={resetAll} aria-label="Restart game">
                                    Back to Selection
                                </button>
                            </div>
                        </div>

                        <div className="battle-log" aria-live="polite">
                            <ul>
                                {log.map((l, i) => (
                                    <li key={i}>{l}</li>
                                ))}
                            </ul>
                        </div>
                    </div>
                )}

                {gameState === 'victory' && (
                    <div className="card victory-card">
                        <div className="throne-room">
                            <svg width="220" height="160" viewBox="0 0 220 160" aria-hidden="true">
                                <rect x="0" y="0" width="220" height="160" fill="#2b2b39" rx="12" />
                                <rect x="40" y="30" width="140" height="90" fill="#111" rx="6" />
                                <g transform="translate(55,40)">
                                    <rect x="0" y="0" width="110" height="70" fill="#7c4dff" rx="8" />
                                    <circle cx="55" cy="30" r="18" fill="#ffd27f" />
                                </g>
                            </svg>
                        </div>
                        <h2 className="section-title">You have beaten the game</h2>
                        <p>All opponents have been defeated. The champion sits upon the throne.</p>
                        <div className="controls-right">
                            <button className="btn reset" onClick={resetAll}>Play Again</button>
                        </div>
                    </div>
                )}

                {gameState === 'defeat' && (
                    <div className="card defeat-card">
                        <h2 className="section-title">You were defeated</h2>
                        <p>Your champion has fallen. Try again.</p>
                        <div className="controls-right">
                            <button className="btn reset" onClick={resetAll}>Try Again</button>
                        </div>
                    </div>
                )}
            </main>
        </div>
    )
}

export default App;