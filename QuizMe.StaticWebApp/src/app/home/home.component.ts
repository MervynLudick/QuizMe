import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CardModule } from 'primeng/card';
import { TabsModule } from 'primeng/tabs';
import { MessageService } from 'primeng/api';
import { GameService } from '../services/game.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [FormsModule, ButtonModule, InputTextModule, CardModule, TabsModule],
  templateUrl: './home.component.html'
})
export class HomeComponent {
  private gameService = inject(GameService);
  private router = inject(Router);
  private messageService = inject(MessageService);

  hostName = '';
  numberOfPlayers = 2;

  joinRoomCode = '';
  joinName = '';

  loading = signal(false);

  createGame() {
    if (!this.hostName.trim()) {
      this.messageService.add({ severity: 'warn', summary: 'Missing name', detail: 'Enter your name.' });
      return;
    }
    this.loading.set(true);
    this.gameService.createGame(this.numberOfPlayers).subscribe({
      next: ({ roomCode }) => {
        this.gameService.joinGame(roomCode, this.hostName).subscribe({
          next: () => {
            sessionStorage.setItem('playerName', this.hostName);
            sessionStorage.setItem('roomCode', roomCode);
            this.router.navigate(['/lobby', roomCode]);
          },
          error: () => this.loading.set(false)
        });
      },
      error: () => {
        this.loading.set(false);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Could not create game.' });
      }
    });
  }

  joinGame() {
    if (!this.joinName.trim() || !this.joinRoomCode.trim()) {
      this.messageService.add({ severity: 'warn', summary: 'Missing fields', detail: 'Enter room code and your name.' });
      return;
    }
    const roomCode = this.joinRoomCode.toUpperCase();
    this.loading.set(true);
    this.gameService.joinGame(roomCode, this.joinName).subscribe({
      next: () => {
        sessionStorage.setItem('playerName', this.joinName);
        sessionStorage.setItem('roomCode', roomCode);
        this.router.navigate(['/lobby', roomCode]);
      },
      error: (err) => {
        this.loading.set(false);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error ?? 'Could not join game.' });
      }
    });
  }
}
