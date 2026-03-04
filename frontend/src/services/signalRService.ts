import * as signalR from "@microsoft/signalr";
import type { BattleState } from "../types";

class SignalRService {
    private connection: signalR.HubConnection | null = null;
    private reconnectAttempts = 0;
    private maxReconnectAttempts = 3; // Reduced from 5
    private isConnecting = false;

    async connect(): Promise<void> {
        // Prevent multiple simultaneous connection attempts
        if (this.isConnecting) {
            console.log("Connection attempt already in progress");
            return;
        }

        // If already connected, return immediately
        if (this.connection?.state === signalR.HubConnectionState.Connected) {
            console.log("Already connected to SignalR");
            return;
        }

        // If connection exists but not connected, try to reuse it
        if (this.connection?.state === signalR.HubConnectionState.Connecting) {
            console.log("Connection attempt already in progress");
            return;
        }

        // Clean up any existing connection before creating a new one
        if (this.connection) {
            try {
                await this.connection.stop();
                console.log("Cleaned up existing connection");
            } catch (error) {
                console.warn("Error cleaning up connection:", error);
            }
        }

        this.isConnecting = true;

        try {
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl("/gameHub")
                .withAutomaticReconnect({
                    nextRetryDelayInMilliseconds: (retryContext) => {
                        // Exponential backoff with max delay
                        if (retryContext.previousRetryCount < 2) {
                            return 2000;
                        } else if (retryContext.previousRetryCount < 4) {
                            return 5000;
                        } else {
                            return 10000;
                        }
                    }
                })
                .configureLogging(signalR.LogLevel.Information)
                .build();

            this.connection.onreconnecting((error) => {
                console.warn("SignalR reconnecting...", error);
                this.reconnectAttempts++;
            });

            this.connection.onreconnected((connectionId) => {
                console.log("SignalR reconnected:", connectionId);
                this.reconnectAttempts = 0;
            });

            this.connection.onclose((error) => {
                console.error("SignalR connection closed:", error);
                this.connection = null; // Clear the connection reference

                // Only attempt auto-reconnect if under the limit and not a rate limit error
                const is429Error = error?.message?.includes('429') || error?.message?.includes('limit');
                if (!is429Error && this.reconnectAttempts < this.maxReconnectAttempts) {
                    console.log(`Attempting to reconnect in 5 seconds (attempt ${this.reconnectAttempts + 1}/${this.maxReconnectAttempts})`);
                    setTimeout(() => this.connect(), 5000);
                } else if (is429Error) {
                    console.error("Rate limit reached. Please close other tabs and wait a few minutes before trying again.");
                }
            });

            await this.connection.start();
            console.log("SignalR Connected successfully");
            this.isConnecting = false;
            this.reconnectAttempts = 0; // Reset on successful connection
        } catch (error) {
            console.error("Error connecting to SignalR:", error);
            this.isConnecting = false;
            this.connection = null;
            throw error;
        }
    }

    async disconnect(): Promise<void> {
        this.isConnecting = false;
        if (this.connection) {
            try {
                await this.connection.stop();
                this.connection = null;
                console.log("SignalR disconnected and cleaned up");
            } catch (error) {
                console.error("Error disconnecting SignalR:", error);
                this.connection = null;
            }
        }
    }

    async joinMatchmaking(playerName: string, characterId: number): Promise<void> {
        if (!this.connection) throw new Error("Not connected to SignalR");
        
        try {
            await this.connection.invoke("JoinMatchmaking", playerName, characterId);
            console.log(`Joined matchmaking as ${playerName} with character ${characterId}`);
        } catch (error) {
            console.error("Error joining matchmaking:", error);
            throw error;
        }
    }

    async selectAbility(battleId: number, abilityId: number): Promise<void> {
        if (!this.connection) throw new Error("Not connected to SignalR");

        try {
            await this.connection.invoke("SelectAbility", battleId, abilityId);
            console.log(`Selected ability ${abilityId} in battle ${battleId}`);
        } catch (error) {
            console.error("Error selecting ability:", error);
            throw error;
        }
    }

    // Team Battle Methods
    async switchCharacter(battleId: number, newCharacterIndex: number): Promise<void> {
        if (!this.connection) throw new Error("Not connected to SignalR");

        try {
            await this.connection.invoke("SwitchCharacter", battleId, newCharacterIndex);
            console.log(`Switched to character index ${newCharacterIndex} in battle ${battleId}`);
        } catch (error) {
            console.error("Error switching character:", error);
            throw error;
        }
    }

    async joinTeamMatchmaking(playerName: string, characterIds: number[]): Promise<void> {
        if (!this.connection) throw new Error("Not connected to SignalR");

        try {
            await this.connection.invoke("JoinTeamMatchmaking", playerName, characterIds);
            console.log(`Joined team matchmaking as ${playerName} with characters ${characterIds.join(',')}`);
        } catch (error) {
            console.error("Error joining team matchmaking:", error);
            throw error;
        }
    }

    onCharacterSwitched(callback: (data: any) => void): void {
        this.connection?.on("CharacterSwitched", (data: any) => {
            console.log("Character switched:", data);
            callback(data);
        });
    }

    onOpponentSwitched(callback: (data: any) => void): void {
        this.connection?.on("OpponentSwitched", (data: any) => {
            console.log("Opponent switched:", data);
            callback(data);
        });
    }

    offCharacterSwitched(): void {
        this.connection?.off("CharacterSwitched");
    }

    offOpponentSwitched(): void {
        this.connection?.off("OpponentSwitched");
    }

    onMatchmakingStatus(callback: (status: string, queueSize: number) => void): void {
        this.connection?.on("MatchmakingStatus", (status: string, queueSize: number) => {
            console.log("Matchmaking status:", status, "Queue size:", queueSize);
            callback(status, queueSize);
        });
    }

    onMatchFound(callback: (battleId: number, opponentName: string, opponentCharacterId: number, isMyTurn: boolean, opponentTeam?: any[]) => void): void {
        this.connection?.on("MatchFound", (battleId: number, opponentName: string, opponentCharacterId: number, isMyTurn: boolean, opponentTeam?: any[]) => {
            console.log("Match found! Battle:", battleId, "Opponent:", opponentName, "My turn:", isMyTurn, "Opponent team:", opponentTeam);
            callback(battleId, opponentName, opponentCharacterId, isMyTurn, opponentTeam);
        });
    }

    onMatchmakingError(callback: (error: string) => void): void {
        this.connection?.on("MatchmakingError", (error: string) => {
            console.error("Matchmaking error:", error);
            callback(error);
        });
    }

    onTurnProcessed(callback: (result: BattleState) => void): void {
        this.connection?.on("TurnProcessed", (result: BattleState) => {
            console.log("Turn processed:", result);
            callback(result);
        });
    }

    onTurnTimeout(callback: (message: string) => void): void {
        this.connection?.on("TurnTimeout", (message: string) => {
            console.warn("Turn timeout:", message);
            callback(message);
        });
    }

    onError(callback: (error: string) => void): void {
        this.connection?.on("Error", (error: string) => {
            console.error("SignalR error:", error);
            callback(error);
        });
    }

    onOpponentReady(callback: (data: { battleId: number; message: string; opponentName: string }) => void): void {
        this.connection?.on("OpponentReady", (data: { battleId: number; message: string; opponentName: string }) => {
            console.log("Opponent ready:", data);
            callback(data);
        });
    }

    offMatchmakingStatus(): void {
        this.connection?.off("MatchmakingStatus");
    }

    offMatchFound(): void {
        this.connection?.off("MatchFound");
    }

    offTurnProcessed(): void {
        this.connection?.off("TurnProcessed");
    }

    offOpponentReady(): void {
        this.connection?.off("OpponentReady");
    }

    offMatchmakingError(): void {
        this.connection?.off("MatchmakingError");
    }

    offTurnTimeout(): void {
        this.connection?.off("TurnTimeout");
    }

    offError(): void {
        this.connection?.off("Error");
    }

    getConnectionState(): signalR.HubConnectionState | null {
        return this.connection?.state ?? null;
    }

    isConnected(): boolean {
        return this.connection?.state === signalR.HubConnectionState.Connected;
    }
}

export const signalRService = new SignalRService();
