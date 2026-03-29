import { Component, DestroyRef, OnInit, inject, signal, computed } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { timer } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { TagModule } from 'primeng/tag';
import { DividerModule } from 'primeng/divider';
import { GameService } from '../services/game.service';
import { Game } from '../models/game.models';

@Component({
  selector: 'app-game',
  standalone: true,
  imports: [FormsModule, CardModule, ButtonModule, InputTextModule, ProgressSpinnerModule, TagModule, DividerModule],
  templateUrl: './game.component.html'
})
export class GameComponent implements OnInit {
  private gameService = inject(GameService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private destroyRef = inject(DestroyRef);

  roomCode = this.route.snapshot.paramMap.get('roomCode')!;
  playerName = sessionStorage.getItem('playerName') ?? '';
  game = signal<Game | null>(null);
  answerInput = '';
  submitting = signal(false);

  currentQuestion = computed(() => {
    const g = this.game();
    return g ? (g.questions[g.currentQuestion] ?? null) : null;
  });

  hasAnswered = computed(() => {
    const g = this.game();
    if (!g) return false;
    const q = g.questions[g.currentQuestion];
    if (!q) return false;
    return g.answers.some(a => a.player.name === this.playerName && a.question.text === q.text);
  });

  ngOnInit() {
    timer(0, 2000).pipe(
      switchMap(() => this.gameService.getGame(this.roomCode)),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(game => {
      this.game.set(game);
      if (game.isGameOver) {
        this.router.navigate(['/results', this.roomCode]);
      }
    });
  }

  submitAnswer() {
    const g = this.game();
    if (!g || !this.answerInput.trim() || this.submitting()) return;
    this.submitting.set(true);
    this.gameService.submitAnswer(this.roomCode, this.playerName, this.answerInput.trim(), g.currentQuestion).subscribe({
      next: (updated) => {
        this.game.set(updated);
        this.answerInput = '';
        this.submitting.set(false);
        if (updated.isGameOver) {
          this.router.navigate(['/results', this.roomCode]);
        }
      },
      error: () => this.submitting.set(false)
    });
  }
}
