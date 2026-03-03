import * as signalR from "@microsoft/signalr";
import type { BattleState } from "../types";

class SignalRService {
    private connection: signalR.HubConnection | null = null;
    private reconnectAttempts = 0;
    private maxReconnectAttempts = 5;

    async connect(): Promise<void> {
        if (this.connection?.state === signalR.HubConnectionState.Connected) {
            console.log("Already connected to SignalR");
            return;
        }

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/gameHub")
            .withAutomaticReconnect({
                nextRetryDelayInMilliseconds: (retryContext) => {
                    if (retryContext.previousRetryCount < 3) {
                        return 2000;
                    } else if (retryContext.previousRetryCount < 6) {
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
            if (this.reconnectAttempts < this.maxReconnectAttempts) {
                setTimeout(() => this.connect(), 5000);
            }
        });

        try {
            await this.connection.start();
            console.log("SignalR Connected successfully");
        } catch (error) {
            console.error("Error connecting to SignalR:", error);
            throw error;
        }
    }

    async disconnect(): Promise<void> {
        if (this.connection) {
            await this.connection.stop();
            console.log("SignalR disconnected");
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

    onMatchmakingStatus(callback: (status: string, queueSize: number) => void): void {
        this.connection?.on("MatchmakingStatus", (status: string, queueSize: number) => {
            console.log("Matchmaking status:", status, "Queue size:", queueSize);
            callback(status, queueSize);
        });
    }

    onMatchFound(callback: (battleId: number, opponentName: string, opponentCharacterId: number, isMyTurn: boolean) => void): void {
        this.connection?.on("MatchFound", (battleId: number, opponentName: string, opponentCharacterId: number, isMyTurn: boolean) => {
            console.log("Match found! Battle:", battleId, "Opponent:", opponentName, "My turn:", isMyTurn);
            callback(battleId, opponentName, opponentCharacterId, isMyTurn);
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

    offMatchmakingStatus(): void {
        this.connection?.off("MatchmakingStatus");
    }

    offMatchFound(): void {
        this.connection?.off("MatchFound");
    }

    offTurnProcessed(): void {
        this.connection?.off("TurnProcessed");
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
