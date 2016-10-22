using System;
using System.ComponentModel.DataAnnotations;

namespace Legomatic.Trading
{
    [CustomValidation(typeof(Brick), "FinalCheck")]
    public class Brick
    {
        public Brick()
        {
        }

        public Brick(int id, DateTime closedOn, decimal open,
            decimal high, decimal low, decimal close, Decimals decimals)
        {
            Id = id;
            ClosedOn = closedOn;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Decimals = decimals;

            ValidationHelper.ValidateObject(this);
        }

        public int Id { get; }

        public DateTime ClosedOn { get; }

        [Required]
        [Range(0.00001, 999.0)]
        public decimal Open { get; }

        [Required]
        [Range(0.00001, 999.0)]
        public decimal High { get; }

        [Required]
        [Range(0.00001, 999.0)]
        public decimal Low { get; }

        [Required]
        [Range(0.00001, 999.0)]
        public decimal Close { get; }

        [Required]
        public Decimals Decimals { get;  }

        public decimal SizeInRate
        {
            get
            {
                return Math.Abs(Math.Round(Close - Open, (int)Decimals));
            }
        }

        public decimal SizeInPips
        {
            get
            {
                return SizeInRate.RateToPips(Decimals);
            }
        }


        public Trend Trend
        {
            get
            {
                if (Open > Close)
                    return Trend.Bear;
                else
                    return Trend.Bull;
            }
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                Id, ClosedOn, Open, High, Low, Close, Trend, Decimals);
        }

        public static ValidationResult FinalCheck(Brick brick, ValidationContext contect)
        {
            if (brick.High < brick.Low)
                return new ValidationResult("The \"High\" must be greater than or equal to the \"Low\"!");

            if (brick.Open < brick.Low)
                return new ValidationResult("The \"Open\" value must be greater than or equal to the \"Low\"!");

            if (brick.Close > brick.High)
                return new ValidationResult("The \"Close\" value must be less than or equal to the \"High\"!");

            return ValidationResult.Success;
        }
    }
}
