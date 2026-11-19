using System;
using System.Threading;

namespace TheEggOfFortune.Logic
{
    public class ScoreService
    {
        public const int MAX_SCORE = 20000;
        private readonly SemaphoreSlim _scoreGate = new(1, 1);
        private int _score = MAX_SCORE;

        public event EventHandler ScoreChanged;

        public int Score
        {
            get
            {
                _scoreGate.Wait();
                int score = _score;
                _scoreGate.Release();
                return score;
            }
        }

        #region Ctor
        public ScoreService()
        {

        }
        #endregion

        public void AddScore()
        {
            _scoreGate.Wait();
            _score++;
            if (_score > MAX_SCORE)
            {
                _score = MAX_SCORE;
            }
            _scoreGate.Release();
            this.ScoreChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SubtractScore()
        {
            _scoreGate.Wait();
            _score--;
            if (_score < 0)
            {
                _score = 0;
            }
            _scoreGate.Release();
            this.ScoreChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetScore(int score)
        {
            _scoreGate.Wait();
            _score = score;
            if (_score > MAX_SCORE)
            {
                _score = MAX_SCORE;
            }
            if (_score < 0)
            {
                _score = 0;
            }
            _scoreGate.Release();
            this.ScoreChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ResetScore()
        {
            _scoreGate.Wait();
            _score = MAX_SCORE;
            _scoreGate.Release();
            this.ScoreChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
