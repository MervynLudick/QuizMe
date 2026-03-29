import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Game } from '../models/game.models';

@Injectable({ providedIn: 'root' })
export class GameService {
  private http = inject(HttpClient);
  private baseUrl = '/api';

  createGame(numberOfPlayers: number): Observable<{ roomCode: string }> {
    return this.http.post<{ roomCode: string }>(`${this.baseUrl}/games`, { numberOfPlayers });
  }

  joinGame(roomCode: string, playerName: string): Observable<{ playerName: string }> {
    return this.http.post<{ playerName: string }>(`${this.baseUrl}/games/${roomCode}/join`, { playerName });
  }

  getGame(roomCode: string): Observable<Game> {
    return this.http.get<Game>(`${this.baseUrl}/games/${roomCode}`);
  }

  submitAnswer(roomCode: string, playerName: string, answer: string, questionNumber: number): Observable<Game> {
    return this.http.post<Game>(`${this.baseUrl}/games/${roomCode}/answer`, { playerName, answer, questionNumber });
  }
}
