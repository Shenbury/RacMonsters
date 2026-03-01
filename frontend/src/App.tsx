import * as React from 'react'
const { useEffect, useRef, useState } = React
import './App.css'

// Music files located in public/music. Update this list if you add/remove songs.
const SONG_FILENAMES = [
    'RAC Battle Theme(1).mp3',
    'RAC Battle Theme(2).mp3',
    'RAC Battle Theme(3).mp3',
    'RAC Battle Theme.mp3'
]

type GameState = 'select' | 'battle' | 'victory' | 'defeat' | 'leaderboard'

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
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)
    const [filter, setFilter] = useState<string>('')
    const [playerName, setPlayerName] = useState<string>('')
    const [playerHP, setPlayerHP] = useState<number | null>(null)
    const [enemyHP, setEnemyHP] = useState<number | null>(null)
    const [busy, setBusy] = useState(false)
    const [log, setLog] = useState<string[]>(['Welcome to the arena'])
    const [darkMode, setDarkMode] = useState(false)
    const [playerAnim, setPlayerAnim] = useState<string | null>(null)
    const [enemyAnim, setEnemyAnim] = useState<string | null>(null)
    const [playerMiss, setPlayerMiss] = useState(false)
    const [enemyMiss, setEnemyMiss] = useState(false)
    const [playerBubble, setPlayerBubble] = useState<string | null>(null)
    const [enemyBubble, setEnemyBubble] = useState<string | null>(null)
    const mounted = useRef(true)
    const [player, setPlayer] = useState<Character | null>(null)
    const [enemy, setEnemy] = useState<Character | null>(null)
    const [opponents, setOpponents] = useState<Character[]>([])
    const [gameState, setGameState] = useState<GameState>('select')
    const [hoveredChar, setHoveredChar] = useState<Character | null>(null)
    const [initialOpponentsCount, setInitialOpponentsCount] = useState<number>(0)

    // leaderboard entries fetched from backend
    interface LeaderboardEntry { id?: number; name: string; score: number; timestamp?: string }
    const [leaderboard, setLeaderboard] = useState<LeaderboardEntry[]>([])

    // Music player state
    const audioRef = useRef<HTMLAudioElement | null>(null)
    const [trackIndex, setTrackIndex] = useState<number>(0)
    const [isPlaying, setIsPlaying] = useState<boolean>(false)
    const tracks = SONG_FILENAMES.map(n => `/music/${encodeURIComponent(n)}`)

    const fetchLeaderboard = async (limit = 50) => {
        try {
            const res = await fetch(`/api/leaderboard/top?limit=${limit}`)
            if (!res.ok) return
            const data = await res.json()
            setLeaderboard(Array.isArray(data) ? data : [])
        } catch {
            // ignore
        }
    }

    const addLeaderboardEntry = async (name: string, score: number) => {
        if (!name) return
        try {
            await fetch('/api/leaderboard', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ name, score }) })
            // refresh local copy
            fetchLeaderboard()
        } catch {
            // ignore
        }
    }

    const upsertLeaderboard = async (name: string, delta = 1) => {
        if (!name) return
        try {
            // call upsert endpoint using query parameters so simple binding picks them up
            await fetch(`/api/leaderboard/upsert?name=${encodeURIComponent(name)}&delta=${delta}`, { method: 'POST' })
            await fetchLeaderboard()
        } catch {
            // ignore
        }
    }

    useEffect(() => {
        mounted.current = true
        return () => {
            mounted.current = false
        }
    }, [])

    useEffect(() => {
        // persist theme preference
        const stored = localStorage.getItem('rac-dark')
        if (stored != null) setDarkMode(stored === 'true')
        const storedName = localStorage.getItem('rac-player-name')
        if (storedName) setPlayerName(storedName)
    }, [])

    // Initialize audio element once
    useEffect(() => {
        if (tracks.length === 0) return
        const audio = new Audio(tracks[trackIndex])
        audio.preload = 'auto'
        audioRef.current = audio

        const onEnded = () => {
            // auto-advance
            setIsPlaying(false)
            setTrackIndex(i => (i + 1) % tracks.length)
        }

        audio.addEventListener('ended', onEnded)

        // attempt to autoplay on load (may be blocked by browser autoplay policies)
        audio.play().then(() => {
            setIsPlaying(true)
        }).catch(() => {
            // autoplay blocked; remain paused until user interacts
            setIsPlaying(false)
        })

        return () => {
            audio.pause()
            audio.removeEventListener('ended', onEnded)
            audioRef.current = null
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [])

    // update audio src when trackIndex changes
    useEffect(() => {
        const audio = audioRef.current
        if (!audio) return
        audio.src = tracks[trackIndex]
        audio.load()
        if (isPlaying) {
            audio.play().catch(() => {
                // ignore play errors (autoplay restrictions)
                setIsPlaying(false)
            })
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [trackIndex])

    // keep play/pause in sync
    useEffect(() => {
        const audio = audioRef.current
        if (!audio) return
        if (isPlaying) {
            audio.play().catch(() => setIsPlaying(false))
        } else {
            audio.pause()
        }
    }, [isPlaying])

    const playPause = async () => {
        if (!audioRef.current) return
        if (isPlaying) {
            setIsPlaying(false)
        } else {
            try {
                await audioRef.current.play()
                setIsPlaying(true)
            } catch {
                setIsPlaying(false)
            }
        }
    }

    const stop = () => {
        const audio = audioRef.current
        if (!audio) return
        audio.pause()
        audio.currentTime = 0
        setIsPlaying(false)
    }

    const nextTrack = async () => {
        if (tracks.length === 0) return
        setTrackIndex(i => (i + 1) % tracks.length)
        // attempt to play next one
        setTimeout(() => {
            const audio = audioRef.current
            if (!audio) return
            audio.play().then(() => setIsPlaying(true)).catch(() => setIsPlaying(false))
        }, 50)
    }

    useEffect(() => {
        // load leaderboard when app starts
        fetchLeaderboard()
    }, [])

    useEffect(() => {
        document.documentElement.dataset.theme = darkMode ? 'dark' : 'light'
        localStorage.setItem('rac-dark', darkMode ? 'true' : 'false')
    }, [darkMode])

    const pushLog = (s: string) => setLog(l => [s, ...l].slice(0, 6))

    const load = async () => {
        setLoading(true)
        setError(null)
        try {
            const res = await fetch('/api/characters')
            if (!res.ok) throw new Error(`Status ${res.status}`)
            const chars = await res.json()
            if (Array.isArray(chars) && chars.length > 0) {
                setAllChars(chars)
                opponents && opponents.length === 0 && setOpponents(chars) // set opponents if not already set
            }
        } catch (ex: any) {
            const msg = ex?.message ?? 'Failed to load characters'
            setError(msg)
            pushLog('Failed to load characters')
        } finally {
            setLoading(false)
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
        if (!playerName || playerName.trim() === '') {
            pushLog('Please enter a player name before choosing a champion')
            return
        }

        // choose player; opponents are others
        const p = { ...char }
        const others = allChars.filter(c => c.id !== char.id).map(c => ({ ...c }))
        setPlayer(p)
        setOpponents(others)
        setInitialOpponentsCount(others.length)
        initialOpponentsCount === 0 && setInitialOpponentsCount(others.length) // set initial count if not already set
        localStorage.setItem('rac-player-name', playerName)
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

    const handleEnemyDefeated = async (defeatedEnemy: any) => {
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

        // update leaderboard for this single defeated opponent
        try {
            await upsertLeaderboard(playerName, 1)
        } catch {
            // ignore
        }
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
            // show speech bubbles for both actors
            setPlayerBubble(playerAbilityName)
            setTimeout(() => setPlayerBubble(null), 1200)
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
                setEnemyBubble(aiAbilityName)
                setTimeout(() => setEnemyBubble(null), 1200)
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
                // record defeat to leaderboard (score 0)
                addLeaderboardEntry(playerName, 0)
            }
        }

        setBusy(false)
    }

    // keyboard shortcuts for abilities (1-4) and restart (r)
    useEffect(() => {
        const onKey = (e: KeyboardEvent) => {
            if (gameState !== 'battle') return
            if (!player || !player.abilities) return
            if (e.key >= '1' && e.key <= '4') {
                const idx = parseInt(e.key, 10) - 1
                if (player.abilities[idx]) doPlayerAttack(idx)
            }
            if (e.key.toLowerCase() === 'r') resetAll()
        }
        window.addEventListener('keydown', onKey)
        return () => window.removeEventListener('keydown', onKey)
    }, [gameState, player])

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
                <div style={{ display: 'flex', gap: 8, alignItems: 'center' }}>
                    <input placeholder="Your name" aria-label="Player name" value={playerName} onChange={e => setPlayerName(e.target.value)} style={{ padding: 6, borderRadius: 6 }} />
                    <button className="btn" onClick={() => { localStorage.setItem('rac-player-name', playerName); pushLog('Name saved') }}>Save</button>
                    <button className="btn" onClick={() => setGameState('leaderboard')}>Leaderboard</button>
                </div>
            </header>

            {/* Persistent music player in header area */}
            <div className="music-player" role="region" aria-label="Music player">
                <div className="music-controls">
                    <button className="btn" onClick={playPause} aria-label="Play or pause">{isPlaying ? 'Pause' : 'Play'}</button>
                    <button className="btn" onClick={stop} aria-label="Stop">Stop</button>
                    <button className="btn" onClick={nextTrack} aria-label="Next">Next</button>
                </div>
                <div className="music-info">
                    <div className="track-name">{decodeURIComponent(tracks[trackIndex].split('/').pop() ?? '')}</div>
                </div>
            </div>

            <main className="main-content">
                {gameState === 'select' && (
                    <div className="card select-card">
                        <h2 className="section-title">Choose your champion</h2>
                        <div className="section-header">
                            <div style={{ display: 'flex', gap: 8, alignItems: 'center' }}>
                                <input aria-label="Filter characters" placeholder="Search..." value={filter} onChange={e => setFilter(e.target.value)} style={{ padding: 8, borderRadius: 8, border: '1px solid rgba(0,0,0,0.06)' }} />
                                <button className="refresh-button" onClick={load} disabled={loading} aria-label="Reload characters">Reload</button>
                            </div>
                        </div>

                        {loading ? (
                            <div className="loading-skeleton-grid">
                                <div className="skeleton-tile" />
                                <div className="skeleton-tile" />
                                <div className="skeleton-tile" />
                                <div className="skeleton-tile" />
                            </div>
                        ) : error ? (
                            <div>
                                <div className="error-banner">Failed to load: {error}</div>
                            </div>
                        ) : (
                            <div className="char-grid">
                                {allChars
                                    .filter(c => !filter || (c.name ?? '').toString().toLowerCase().includes(filter.toLowerCase()))
                                    .map((c) => (
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
                                                    {c.imageUrl ? (
                                                        <img src={c.imageUrl} alt={c.name} />
                                                    ) : (
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
                        )}
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
                                    {playerBubble && (
                                        <div className="speech-bubble player" aria-hidden="true">{playerBubble}</div>
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
                                    {enemyBubble && (
                                        <div className="speech-bubble enemy" aria-hidden="true">{enemyBubble}</div>
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

export default App