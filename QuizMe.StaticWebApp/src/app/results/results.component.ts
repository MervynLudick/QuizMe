import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { DividerModule } from 'primeng/divider';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { GameService } from '../services/game.service';
import { Game } from '../models/game.models';

@Component({
  selector: 'app-results',
  standalone: true,
  imports: [CardModule, ButtonModule, TagModule, DividerModule, ProgressSpinnerModule, RouterLink],
  templateUrl: './results.component.html'
})
export class ResultsComponent implements OnInit {
  private gameService = inject(GameService);
  private route = inject(ActivatedRoute);

  roomCode = this.route.snapshot.paramMap.get('roomCode')!;
  playerName = sessionStorage.getItem('playerName') ?? '';
  game = signal<Game | null>(null);

  sortedScores = computed(() => {
    const g = this.game();
    if (!g) return [];
    return Object.entries(g.scores)
      .sort(([, a], [, b]) => b - a)
      .map(([name, score]) => ({ name, score }));
  });

  ngOnInit() {
    this.gameService.getGame(this.roomCode).subscribe(game => this.game.set(game));
  }

  isWinner(name: string): boolean {
    return this.game()?.winners.some(w => w.name === name) ?? false;
  }
}
