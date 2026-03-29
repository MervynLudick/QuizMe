import { Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { timer } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { CardModule } from 'primeng/card';
import { ChipModule } from 'primeng/chip';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { GameService } from '../services/game.service';
import { Game } from '../models/game.models';

@Component({
  selector: 'app-lobby',
  standalone: true,
  imports: [CardModule, ChipModule, ProgressSpinnerModule],
  templateUrl: './lobby.component.html'
})
export class LobbyComponent implements OnInit {
  private gameService = inject(GameService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private destroyRef = inject(DestroyRef);

  roomCode = this.route.snapshot.paramMap.get('roomCode')!;
  playerName = sessionStorage.getItem('playerName') ?? '';
  game = signal<Game | null>(null);

  ngOnInit() {
    timer(0, 2000).pipe(
      switchMap(() => this.gameService.getGame(this.roomCode)),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(game => {
      this.game.set(game);
      if (game.players.length >= game.numberOfPlayers) {
        this.router.navigate(['/game', this.roomCode]);
      }
    });
  }
}
