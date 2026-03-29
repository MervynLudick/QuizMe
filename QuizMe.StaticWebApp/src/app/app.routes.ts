import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./home/home.component').then(m => m.HomeComponent)
  },
  {
    path: 'lobby/:roomCode',
    loadComponent: () => import('./lobby/lobby.component').then(m => m.LobbyComponent)
  },
  {
    path: 'game/:roomCode',
    loadComponent: () => import('./game/game.component').then(m => m.GameComponent)
  },
  {
    path: 'results/:roomCode',
    loadComponent: () => import('./results/results.component').then(m => m.ResultsComponent)
  }
];
