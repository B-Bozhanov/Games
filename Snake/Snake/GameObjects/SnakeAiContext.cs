namespace SnakeGame.GameObjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using SnakeGame.GameObjects.Abstractions.Interfaces;

    public readonly struct SnakeAiContext
    {
        public SnakeAiContext(
            Coordinates head,
            Coordinates food,
            IReadOnlyCollection<Coordinates> body,
            IGameBoard gameBoard)
        {
            this.Head = head;
            this.Food = food;
            this.Body = body;
            this.GameBoard = gameBoard;
        }

        public Coordinates Head { get; }

        public Coordinates Food { get; }

        public IReadOnlyCollection<Coordinates> Body { get; }

        public IGameBoard GameBoard { get; }
    }
}
