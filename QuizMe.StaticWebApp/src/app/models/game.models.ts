export interface Player {
  name: string;
}

export interface Question {
  category: string;
  text: string;
  answer: string;
}

export interface PlayerAnswer {
  player: Player;
  question: Question;
  answer: string;
}

export interface Game {
  players: Player[];
  questions: Question[];
  answers: PlayerAnswer[];
  numberOfPlayers: number;
  currentQuestion: number;
  isGameOver: boolean;
  scores: Record<string, number>;
  winners: Player[];
}
