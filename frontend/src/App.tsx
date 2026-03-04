import * as React from 'react'
const { useEffect, useRef, useState } = React
import './App.css'
import { GameModeSelection } from './components/GameModeSelection'
import { MultiplayerLobby } from './components/MultiplayerLobby'
import { CharacterSelection } from './components/CharacterSelection'
import { TeamCharacterSelection } from './components/TeamCharacterSelection'
import { MultiplayerBattle } from './components/MultiplayerBattle'
import { TeamMultiplayerBattle } from './components/TeamMultiplayerBattle'
import type { Character as BaseCharacter, StatusEffect, StatusEffectType } from './types'

// Extended types to handle both camelCase and PascalCase from API
// Using Record<string, any> to allow additional properties from the backend
interface Ability extends Record<string, any> {
    id?: number
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
    speed?: number
    Speed?: number
    accuracy?: number
    Accuracy?: number
}

interface Character extends Record<string, any> {
    id: number
    name?: string
    Name?: string
    imageUrl?: string
    ImageUrl?: string
    currentHealth?: number
    maxHealth?: number
    attack?: number
    defense?: number
    techAttack?: number
    techDefense?: number
    abilities?: Ability[]
    activeStatusEffects?: StatusEffect[]
}

// Music files located in public/music. Update this list if you add/remove songs.
const SONG_FILENAMES = [
    'Breakdown Battle Royale.mp3',
    'RAC Battle Theme(1).mp3',
    'RAC Battle Theme(2).mp3',
    'RAC Battle Theme(3).mp3',
    'RAC Battle Theme.mp3'
]

// Sound effects located in public/sounds. Update this list if you add/remove sounds.
const MISS_SOUND_FILENAMES = [
    'fart.mp3'
]

const HEAL_SOUND_FILENAMES = [
    'healpop.mp3'
]

const DEFEAT_SOUND_FILENAMES = [
    'smack.mp3',
    'whip.mp3',
    'EmoDamage.mp3'
]

type GameState = 'start' | 'modeselect' | 'select' | 'multiplayerlobby' | 'multiplayerbattle' | 'battle' | 'victory' | 'defeat' | 'leaderboard' | 'transition'

const App: React.FC = () => {
    const [allChars, setAllChars] = useState<Character[]>([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)
    const [playerName, setPlayerName] = useState<string>('')
    const [nameSaved, setNameSaved] = useState<boolean>(false)
    const [playerHP, setPlayerHP] = useState<number | null>(null)
    const [enemyHP, setEnemyHP] = useState<number | null>(null)
    const [busy, setBusy] = useState(false)
    const [log, setLog] = useState<string[]>(['Welcome to the arena'])
    const [darkMode, setDarkMode] = useState(true)
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
    const [gameState, setGameState] = useState<GameState>('start')
    const [gameMode, setGameMode] = useState<'singleplayer' | 'multiplayer' | 'teambattle' | null>(null)
    const [initialOpponentsCount, setInitialOpponentsCount] = useState<number>(0)
    const [defeatedEnemy, setDefeatedEnemy] = useState<Character | null>(null)
    const [nextEnemy, setNextEnemy] = useState<Character | null>(null)

    // Multiplayer battle state
    const [multiplayerBattleId, setMultiplayerBattleId] = useState<number | null>(null)
    const [multiplayerOpponentName, setMultiplayerOpponentName] = useState<string>('')
    const [multiplayerOpponentCharId, setMultiplayerOpponentCharId] = useState<number | null>(null)
    const [multiplayerIsMyTurn, setMultiplayerIsMyTurn] = useState<boolean>(false)
    const [multiplayerSelectedCharacter, setMultiplayerSelectedCharacter] = useState<Character | null>(null)

    // Team Battle state
    const [selectedTeam, setSelectedTeam] = useState<Character[]>([])

    // leaderboard entries fetched from backend
    interface LeaderboardEntry { id?: number; name: string; character?: string; score: number; timestamp?: string }
    const [leaderboard, setLeaderboard] = useState<LeaderboardEntry[]>([])

    // Music player state
    const audioRef = useRef<HTMLAudioElement | null>(null)
    const [trackIndex, setTrackIndex] = useState<number>(0)
    const [isPlaying, setIsPlaying] = useState<boolean>(false)
    const [volume, setVolume] = useState<number>(0.8)
    const [hasInteracted, setHasInteracted] = useState<boolean>(false)
    const tracks = SONG_FILENAMES.map(n => `/music/${encodeURIComponent(n)}`)

    const doNothing = async () => {
        {
            console.log(setNextEnemy);
            console.log(setDefeatedEnemy);
            console.log(opponents);
            console.log(nameSaved);
        }
    }

    doNothing();

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

    const upsertLeaderboard = async (name: string, delta = 1, character?: string) => {
        if (!name) return
        try {
            // call upsert endpoint using query parameters so simple binding picks them up
            const charParam = character ?? (player?.name ?? '')
            await fetch(`/api/leaderboard/upsert?name=${encodeURIComponent(name)}&delta=${delta}&character=${encodeURIComponent(charParam)}`, { method: 'POST' })
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
        if (storedName) {
            setPlayerName(storedName)
            setNameSaved(true)
            // Skip start screen and go to mode select
            setGameState('modeselect')
        }
        const storedVol = localStorage.getItem('rac-volume')
        if (storedVol != null) {
            const v = parseFloat(storedVol)
            if (!Number.isNaN(v) && v >= 0 && v <= 1) setVolume(v)
        }
    }, [])

    // Initialize audio element once
    useEffect(() => {
        if (tracks.length === 0) return
        const audio = new Audio(tracks[trackIndex])
        audio.preload = 'auto'
        audio.volume = volume
        audioRef.current = audio

        const onEnded = () => {
            // auto-advance and resume playback on the next track
            setTrackIndex(i => (i + 1) % tracks.length)
            // mark playing so the trackIndex effect will attempt to play the new src
            setIsPlaying(true)
        }

        audio.addEventListener('ended', onEnded)

        return () => {
            audio.pause()
            audio.removeEventListener('ended', onEnded)
            audioRef.current = null
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [])

    // Play audio on first user interaction
    useEffect(() => {
        if (hasInteracted) return

        const handleFirstInteraction = () => {
            setHasInteracted(true)
            const audio = audioRef.current
            if (audio && !isPlaying) {
                audio.play().then(() => {
                    setIsPlaying(true)
                }).catch(() => {
                    // ignore play errors
                })
            }
        }

        // listen for any user interaction
        const events = ['click', 'keydown', 'touchstart']
        events.forEach(event => {
            document.addEventListener(event, handleFirstInteraction, { once: true })
        })

        return () => {
            events.forEach(event => {
                document.removeEventListener(event, handleFirstInteraction)
            })
        }
    }, [hasInteracted, isPlaying])

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

    // keep volume in sync with audio element
    useEffect(() => {
        const audio = audioRef.current
        if (!audio) return
        audio.volume = volume
        try {
            localStorage.setItem('rac-volume', String(volume))
        } catch {
            // ignore storage errors
        }
    }, [volume])

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

    // refresh leaderboard whenever the user navigates to the leaderboard view
    useEffect(() => {
        if (gameState === 'leaderboard') fetchLeaderboard()
    }, [gameState])

    useEffect(() => {
        document.documentElement.dataset.theme = darkMode ? 'dark' : 'light'
        localStorage.setItem('rac-dark', darkMode ? 'true' : 'false')
    }, [darkMode])

    const pushLog = (s: string) => setLog(l => [s, ...l].slice(0, 6))

    const playMissSound = () => {
        try {
            const randomIndex = Math.floor(Math.random() * MISS_SOUND_FILENAMES.length)
            const soundFile = MISS_SOUND_FILENAMES[randomIndex]
            const sound = new Audio(`/sounds/${encodeURIComponent(soundFile)}`)
            sound.volume = 0.5
            sound.play().catch(() => {
                // ignore play errors
            })
        } catch {
            // ignore audio errors
        }
    }

    const playHealSound = () => {
        try {
            const randomIndex = Math.floor(Math.random() * HEAL_SOUND_FILENAMES.length)
            const soundFile = HEAL_SOUND_FILENAMES[randomIndex]
            const sound = new Audio(`/sounds/${encodeURIComponent(soundFile)}`)
            sound.volume = 0.5
            sound.play().catch(() => {
                // ignore play errors
            })
        } catch {
            // ignore audio errors
        }
    }

    const playDefeatSound = () => {
        try {
            const randomIndex = Math.floor(Math.random() * DEFEAT_SOUND_FILENAMES.length)
            const soundFile = DEFEAT_SOUND_FILENAMES[randomIndex]
            const sound = new Audio(`/sounds/${encodeURIComponent(soundFile)}`)
            sound.volume = 0.5
            sound.play().catch(() => {
                // ignore play errors
            })
        } catch {
            // ignore audio errors
        }
    }

    const load = async () => {
        setLoading(true)
        setError(null)
        try {
            const res = await fetch('/api/characters')
            if (!res.ok) throw new Error(`Status ${res.status}`)
            const chars = await res.json()
            if (Array.isArray(chars) && chars.length > 0) {
                setAllChars(chars)
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
            const msg = 'Please enter a player name before choosing a champion'
            // surface the message so it's visible on the selection screen
            setError(msg)
            // also add to the transient log for when the battle view appears
            pushLog(msg)
            // clear the error banner after a short delay
            setTimeout(() => {
                // avoid clearing other real errors if they occurred; only clear if still our message
                setError(prev => (prev === msg ? null : prev))
            }, 3500)
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
                    playDefeatSound();
                }
            }
            return updated
        })

        // update leaderboard for this single defeated opponent (record champion used)
        try {
            await upsertLeaderboard(playerName, 1, player?.name)
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
                playMissSound()
            } else if (pHeal) {
                setPlayerAnim('glow-green')
                clearPlayerAnim(1000)
                playHealSound()
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
                playDefeatSound()
                handleEnemyDefeated(enemy)
            }

            if ((newPlayerHP ?? 0) <= 0) {
                setGameState('defeat')
                pushLog(`${player.name} has fallen...`)
                playDefeatSound()
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
        setGameState('start')
        setLog(['Welcome to the arena'])
        setPlayerName('')
        setNameSaved(false)
    }

    const playerMax = player?.maxHealth ?? 100
    const playerHPVal = playerHP ?? playerMax
    const playerHPPercent = Math.max(0, Math.min(100, Math.round((playerHPVal / playerMax) * 100)))

    const enemyMax = enemy?.maxHealth ?? 100
    const enemyHPVal = enemyHP ?? enemyMax
    const enemyHPPercent = Math.max(0, Math.min(100, Math.round((enemyHPVal / enemyMax) * 100)))

    const getStatusEffectIcon = (type: StatusEffectType): string => {
        switch (type) {
            case 0: return '🔥'; // Burn
            case 1: return '☠️'; // Poison
            case 2: return '🩸'; // Bleed
            case 3: return '⚔️↑'; // AttackUp
            case 4: return '⚔️↓'; // AttackDown
            case 5: return '🛡️↑'; // DefenseUp
            case 6: return '🛡️↓'; // DefenseDown
            case 7: return '⚡↑'; // TechAttackUp
            case 8: return '⚡↓'; // TechAttackDown
            case 9: return '🔰↑'; // TechDefenseUp
            case 10: return '🔰↓'; // TechDefenseDown
            case 11: return '🎯↑'; // AccuracyUp
            case 12: return '🎯↓'; // AccuracyDown
            case 13: return '💨↑'; // EvasionUp
            case 14: return '💨↓'; // EvasionDown
            case 15: return '⚡'; // Charging
            case 16: return '🚫'; // HealBlock
            case 17: return '💫'; // Stunned
            case 18: return '✨'; // Protected
            default: return '❓';
        }
    };

    const getStatusEffectTooltip = (effect: StatusEffect): string => {
        const duration = effect.duration;
        const turnText = duration === 1 ? 'turn' : 'turns';

        switch (effect.type) {
            case 0: return `Burn: ${effect.power} damage per turn for ${duration} ${turnText}`;
            case 1: return `Poison: ${effect.power} damage per turn for ${duration} ${turnText}`;
            case 2: return `Bleed: ${effect.power} damage per turn for ${duration} ${turnText}`;
            case 3: return `Attack increased by ${Math.round(effect.modifier * 100)}% for ${duration} ${turnText}`;
            case 4: return `Attack lowered by ${Math.round(effect.modifier * 100)}% for ${duration} ${turnText}`;
            case 5: return `Defense increased by ${Math.round(effect.modifier * 100)}% for ${duration} ${turnText}`;
            case 6: return `Defense lowered by ${Math.round(effect.modifier * 100)}% for ${duration} ${turnText}`;
            case 7: return `Tech Attack increased by ${Math.round(effect.modifier * 100)}% for ${duration} ${turnText}`;
            case 8: return `Tech Attack lowered by ${Math.round(effect.modifier * 100)}% for ${duration} ${turnText}`;
            case 9: return `Tech Defense increased by ${Math.round(effect.modifier * 100)}% for ${duration} ${turnText}`;
            case 10: return `Tech Defense lowered by ${Math.round(effect.modifier * 100)}% for ${duration} ${turnText}`;
            case 11: return `Accuracy increased by ${Math.round(effect.modifier * 100)}% for ${duration} ${turnText}`;
            case 12: return `Accuracy lowered by ${Math.round(effect.modifier * 100)}% for ${duration} ${turnText}`;
            case 13: return `Evasion increased by ${Math.round(effect.modifier * 100)}% for ${duration} ${turnText}`;
            case 14: return `Evasion lowered by ${Math.round(effect.modifier * 100)}% for ${duration} ${turnText}`;
            case 15: return effect.sourceAbilityName 
                ? `Charging ${effect.sourceAbilityName} for ${duration} ${turnText}` 
                : `Charging for ${duration} ${turnText}`;
            case 16: return `Heal Block: Cannot heal for ${duration} ${turnText}`;
            case 17: return `Stunned: Cannot act for ${duration} ${turnText}`;
            case 18: return `Protected: Immune to damage for ${duration} ${turnText}`;
            default: return `${effect.name} for ${duration} ${turnText}`;
        }
    };

    return (
        <div className={`app-container battle-theme ${darkMode ? 'dark' : ''}`}>
            <header className="app-header">
                <h1 className="app-title">RAC Battle</h1>
                <div className="header-controls">
                    {gameState !== 'start' && playerName && (
                        <div className="saved-name">{playerName}</div>
                    )}
                    <label className="dark-toggle">
                        <input
                            type="checkbox"
                            checked={darkMode}
                            onChange={e => setDarkMode(e.target.checked)}
                            aria-label="Toggle dark mode"
                        />
                        <span>Dark</span>
                    </label>
                    {gameState !== 'start' && (
                        <button className="btn" onClick={() => setGameState('leaderboard')}>Leaderboard</button>
                    )}
                    {gameState === 'battle' && (
                        <button className="btn reset" onClick={resetAll} aria-label="Back to selection">
                            Back to Selection
                        </button>
                    )}
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
                    <div className="volume-control" style={{ display: 'flex', alignItems: 'center', gap: 8, marginTop: 6 }}>
                        <input
                            type="range"
                            min={0}
                            max={1}
                            step={0.01}
                            value={volume}
                            onChange={e => setVolume(parseFloat(e.target.value))}
                            aria-label="Volume"
                        />
                        <div style={{ minWidth: 40, textAlign: 'right' }} aria-hidden>{Math.round(volume * 100)}%</div>
                    </div>
                </div>
            </div>

            <main className="main-content">
                {gameState === 'start' && (
                    <div className="card start-card">
                        <h2 className="section-title">Welcome to RAC Battle!</h2>
                        <div className="start-content">
                            <div className="start-logo">
                                <svg width="160" height="160" viewBox="0 0 100 100" aria-hidden="true">
                                    <circle cx="50" cy="50" r="44" fill="#ff9f1c" />
                                    <circle cx="50" cy="40" r="22" fill="#fff" />
                                    <circle cx="42" cy="38" r="4" fill="#111" />
                                    <circle cx="58" cy="38" r="4" fill="#111" />
                                    <path d="M 40 48 Q 50 54 60 48" stroke="#111" strokeWidth="3" fill="none" strokeLinecap="round" />
                                </svg>
                            </div>
                            <p className="start-subtitle">Enter your leaderboard name to begin your journey</p>
                            <div className="start-form">
                                <input
                                    type="text"
                                    placeholder="Enter your name"
                                    aria-label="Player name"
                                    value={playerName}
                                    onChange={e => setPlayerName(e.target.value)}
                                    onKeyDown={e => {
                                        if (e.key === 'Enter' && playerName.trim()) {
                                            localStorage.setItem('rac-player-name', playerName)
                                            setNameSaved(true)
                                            setGameState('modeselect')
                                            pushLog(`Welcome, ${playerName}!`)
                                        }
                                    }}
                                    className="start-input"
                                    autoFocus
                                />
                                <button
                                    className="btn start-button"
                                    onClick={() => {
                                        if (playerName.trim()) {
                                            localStorage.setItem('rac-player-name', playerName)
                                            setNameSaved(true)
                                            setGameState('modeselect')
                                            pushLog(`Welcome, ${playerName}!`)
                                        }
                                    }}
                                    disabled={!playerName.trim()}
                                >
                                    Start Game
                                </button>
                            </div>
                            <button
                                className="btn secondary-btn"
                                onClick={() => setGameState('leaderboard')}
                                style={{ marginTop: '1rem' }}
                            >
                                View Leaderboard
                            </button>
                        </div>
                    </div>
                )}

                {gameState === 'modeselect' && (
                    <GameModeSelection
                        playerName={playerName}
                        onSelectMode={(mode) => {
                            setGameMode(mode);
                            setGameState('select');
                            if (mode === 'singleplayer') {
                                pushLog('Starting single player mode...');
                            } else if (mode === 'teambattle') {
                                pushLog('Select your team of 4 characters...');
                            } else {
                                pushLog('Select your character for multiplayer...');
                            }
                        }}
                    />
                )}

                {gameState === 'multiplayerlobby' && (multiplayerSelectedCharacter || selectedTeam.length > 0) && (
                    <MultiplayerLobby
                        playerName={playerName}
                        selectedCharacter={gameMode === 'teambattle' ? undefined : (multiplayerSelectedCharacter as BaseCharacter)}
                        selectedTeam={gameMode === 'teambattle' ? selectedTeam as BaseCharacter[] : undefined}
                        isTeamBattle={gameMode === 'teambattle'}
                        onMatchFound={(battleId, opponentName, opponentCharacterId, isMyTurn) => {
                            console.log('Match found callback:', {
                                battleId,
                                opponentName,
                                opponentCharacterId,
                                isMyTurn,
                                gameMode,
                                selectedTeamLength: selectedTeam.length,
                                hasMultiplayerChar: !!multiplayerSelectedCharacter
                            });
                            setMultiplayerBattleId(battleId);
                            setMultiplayerOpponentName(opponentName);
                            setMultiplayerOpponentCharId(opponentCharacterId);
                            setMultiplayerIsMyTurn(isMyTurn);
                            setGameState('multiplayerbattle');
                            pushLog(`Match found! You're battling ${opponentName}!`);
                        }}
                        onBack={() => {
                            setMultiplayerSelectedCharacter(null);
                            setSelectedTeam([]);
                            setGameState('select');
                        }}
                    />
                )}

                {gameState === 'select' && (
                    <div className="card select-card">
                        {gameMode === 'teambattle' ? (
                            <TeamCharacterSelection
                                characters={allChars}
                                loading={loading}
                                error={error}
                                onSelectTeam={(team) => {
                                    setSelectedTeam(team);
                                    setGameState('multiplayerlobby');
                                    pushLog(`Team selected! Entering matchmaking...`);
                                }}
                                onBack={() => setGameState('modeselect')}
                                onRefresh={load}
                            />
                        ) : (
                            <CharacterSelection
                                characters={allChars}
                                loading={loading}
                                error={error}
                                onSelectCharacter={(char) => {
                                    if (gameMode === 'singleplayer') {
                                        startWithPlayer(char);
                                    } else if (gameMode === 'multiplayer') {
                                        setMultiplayerSelectedCharacter(char);
                                        setGameState('multiplayerlobby');
                                        pushLog('Entering multiplayer matchmaking...');
                                    }
                                }}
                                onBack={() => setGameState('modeselect')}
                                onRefresh={load}
                                title={gameMode === 'multiplayer' ? 'Choose your champion for multiplayer' : 'Choose your champion'}
                                subtitle={gameMode === 'multiplayer' ? 'Select a character to enter matchmaking' : undefined}
                            />
                        )}
                    </div>
                )}

                {gameState === 'battle' && (
                    <div className="card battle-card" role="region" aria-label="Battle arena">
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
                                    {/* Player Status Effects */}
                                    {player?.activeStatusEffects && player.activeStatusEffects.length > 0 && (
                                        <div className="status-effects">
                                            {player.activeStatusEffects.map((effect, idx) => (
                                                <div key={idx} className="status-effect" title={getStatusEffectTooltip(effect)}>
                                                    <span className="status-icon">{getStatusEffectIcon(effect.type)}</span>
                                                    <span className="status-duration">{effect.duration}</span>
                                                </div>
                                            ))}
                                        </div>
                                    )}
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
                                    {/* Enemy Status Effects */}
                                    {enemy?.activeStatusEffects && enemy.activeStatusEffects.length > 0 && (
                                        <div className="status-effects">
                                            {enemy.activeStatusEffects.map((effect, idx) => (
                                                <div key={idx} className="status-effect" title={getStatusEffectTooltip(effect)}>
                                                    <span className="status-icon">{getStatusEffectIcon(effect.type)}</span>
                                                    <span className="status-duration">{effect.duration}</span>
                                                </div>
                                            ))}
                                        </div>
                                    )}
                                </div>
                            </div>
                        </div>

                        {/* Enemy Abilities - Fixed Sidebar on Right */}
                        {enemy?.abilities && enemy.abilities.length > 0 && (
                            <div className="enemy-abilities-sidebar-fixed">
                                <h4>{enemy.name}'s Abilities</h4>
                                <div className="enemy-abilities-list">
                                    {enemy.abilities.map((a: Ability, i: number) => {
                                        const name = a.name ?? a.Name ?? 'Ability';
                                        const description = a.description ?? a.Description ?? '';
                                        const isTech = a.isTech ?? a.IsTech;
                                        const isHeal = a.isHeal ?? a.IsHeal;
                                        return (
                                            <div key={i} className="enemy-ability-item">
                                                <div className="enemy-ability-header">
                                                    <span className="enemy-ability-name">{name}</span>
                                                    <span className="enemy-ability-type">
                                                        {isHeal ? '🔵' : isTech ? '⚡' : '👊'}
                                                    </span>
                                                </div>
                                                {description && (
                                                    <div className="enemy-ability-description">{description}</div>
                                                )}
                                            </div>
                                        );
                                    })}
                                </div>
                            </div>
                        )}

                        <div className="controls">
                            <div className="actions ability-grid">
                                {player?.abilities && player.abilities.map((a: Ability, i: number) => {
                                    const name = a.name ?? a.Name ?? 'Ability'
                                    const power = a.power ?? a.Power
                                    const description = a.description ?? a.Description ?? ''
                                    const isTech = a.isTech ?? a.IsTech
                                    const isHeal = a.isHeal ?? a.IsHeal
                                    const accuracy = a.accuracy ?? a.Accuracy
                                    return (
                                        <div key={i} className="ability-block">
                                            <button className="btn ability" onClick={() => doPlayerAttack(i)} disabled={busy} aria-label={`Use ${name}`}>
                                                {name} {power ? `(${power})` : ''}
                                            </button>
                                            <div className="ability-meta">
                                                {description && <div className="ability-desc">{description}</div>}
                                                <div className="ability-tags">
                                                    {isTech ? 'Tech' : isHeal ? 'Heal' : ''}
                                                    {(isTech || isHeal) && power ? ` • Power: ${power}` : (!isTech && !isHeal && power ? `Power: ${power}` : '')}
                                                    {accuracy != null ? ` • Acc: ${Math.round(accuracy * 100)}%` : ''}
                                                </div>
                                            </div>
                                        </div>
                                    )
                                })}
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

                {gameState === 'multiplayerbattle' && multiplayerBattleId && gameMode === 'teambattle' && selectedTeam.length > 0 && (
                    <TeamMultiplayerBattle
                        battleId={multiplayerBattleId}
                        opponentName={multiplayerOpponentName}
                        playerTeam={selectedTeam as BaseCharacter[]}
                        onBattleEnd={(won: boolean) => {
                            if (won) {
                                pushLog(`Victory! Your team defeated ${multiplayerOpponentName}!`);
                            } else {
                                pushLog(`Defeat! ${multiplayerOpponentName}'s team was victorious.`);
                            }
                            setMultiplayerBattleId(null);
                            setMultiplayerOpponentName('');
                            setMultiplayerOpponentCharId(null);
                            setSelectedTeam([]);
                            setGameState('modeselect');
                        }}
                        onBackToLobby={() => {
                            setMultiplayerBattleId(null);
                            setMultiplayerOpponentName('');
                            setMultiplayerOpponentCharId(null);
                            setSelectedTeam([]);
                            setGameState('select');
                        }}
                    />
                )}

                {gameState === 'multiplayerbattle' && multiplayerBattleId && multiplayerSelectedCharacter && multiplayerOpponentCharId && (
                    <MultiplayerBattle
                        battleId={multiplayerBattleId}
                        opponentName={multiplayerOpponentName}
                        opponentCharacterId={multiplayerOpponentCharId}
                        initialIsMyTurn={multiplayerIsMyTurn}
                        playerCharacter={multiplayerSelectedCharacter as BaseCharacter}
                        onBattleEnd={(won: boolean) => {
                            if (won) {
                                pushLog(`Victory! You defeated ${multiplayerOpponentName}!`);
                            } else {
                                pushLog(`Defeat! ${multiplayerOpponentName} was victorious.`);
                            }
                            setMultiplayerBattleId(null);
                            setMultiplayerOpponentName('');
                            setMultiplayerOpponentCharId(null);
                            setMultiplayerSelectedCharacter(null);
                            setGameState('modeselect');
                        }}
                        onBackToLobby={() => {
                            setMultiplayerBattleId(null);
                            setMultiplayerOpponentName('');
                            setMultiplayerOpponentCharId(null);
                            setMultiplayerSelectedCharacter(null);
                            setGameState('select');
                        }}
                    />
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

                {gameState === 'leaderboard' && (
                    <div className="card leaderboard-card">
                        <h2 className="section-title">Leaderboard</h2>
                        <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
                            {leaderboard.length === 0 ? (
                                <div>No entries yet</div>
                            ) : (
                                <ol>
                                    {leaderboard.map((e) => (
                                        <li key={e.id ?? `${e.name}-${e.timestamp}`}>{e.name}{e.character ? ` (${e.character})` : ''} — {e.score}</li>
                                    ))}
                                </ol>
                            )}
                            <div className="controls-right">
                                <button className="btn" onClick={() => setGameState('select')}>Back</button>
                            </div>
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

                {gameState === 'transition' && (
                    <div className="card transition-card">
                        <h2 className="section-title">{defeatedEnemy?.name} Defeated!</h2>
                        <div style={{ textAlign: 'center', fontSize: '1.2rem', marginTop: '20px' }}>
                            <p>{nextEnemy?.name} is approaching...</p>
                            <div style={{ marginTop: '20px', fontSize: '0.9rem', opacity: 0.7 }}>
                                Prepare yourself!
                            </div>
                        </div>
                    </div>
                )}
            </main>
        </div>
    )
}

export default App