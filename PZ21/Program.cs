using System;

namespace MoneyExample
{
    class Money
    {
        // Публичные поля для доступа
        public int Rubles { get; set; }
        public int Kopeks { get; set; }

        // Дополнительные поля
        public string Currency { get; set; }
        public string TransactionID { get; set; }
        public string Description { get; set; }

        // Конструктор с нормализацией суммы
        public Money(int rubles, int kopeks, string currency = "RUB", string transactionID = "", string description = "")
        {
            int totalKopeks = rubles * 100 + kopeks;
            int absTotal = Math.Abs(totalKopeks);
            Rubles = totalKopeks >= 0 ? absTotal / 100 : -(absTotal / 100);
            Kopeks = absTotal % 100;
            Currency = currency;
            TransactionID = transactionID;
            Description = description;
        }

        // Копирующий конструктор
        public Money(Money other)
        {
            Rubles = other.Rubles;
            Kopeks = other.Kopeks;
            Currency = other.Currency;
            TransactionID = other.TransactionID;
            Description = other.Description;
        }

        // Метод для расчёта суммы перевода с комиссией
        public Money TransferCost(double commissionPercentage)
        {
            int totalKopeks = Rubles * 100 + Kopeks;
            double fullCost = totalKopeks * (1 + commissionPercentage / 100);
            int roundedKopeks = (int)Math.Round(fullCost, MidpointRounding.AwayFromZero);
            return new Money(roundedKopeks / 100, roundedKopeks % 100, Currency, TransactionID, Description);
        }

        // Перегрузка оператора сложения
        public static Money operator +(Money a, Money b)
        {
            if (a.Currency != b.Currency)
                throw new InvalidOperationException("Нельзя складывать суммы в разных валютах.");
            int totalA = a.Rubles * 100 + a.Kopeks;
            int totalB = b.Rubles * 100 + b.Kopeks;
            int sum = totalA + totalB;
            return new Money(sum / 100, sum % 100, a.Currency, a.TransactionID, a.Description);
        }

        // Перегрузка оператора вычитания
        public static Money operator -(Money a, Money b)
        {
            if (a.Currency != b.Currency)
                throw new InvalidOperationException("Нельзя вычитать суммы в разных валютах.");
            int totalA = a.Rubles * 100 + a.Kopeks;
            int totalB = b.Rubles * 100 + b.Kopeks;
            int diff = totalA - totalB;
            return new Money(diff / 100, diff % 100, a.Currency, a.TransactionID, a.Description);
        }

        // Перегрузка оператора умножения на число
        public static Money operator *(Money a, double multiplier)
        {
            int total = a.Rubles * 100 + a.Kopeks;
            double result = total * multiplier;
            int rounded = (int)Math.Round(result, MidpointRounding.AwayFromZero);
            return new Money(rounded / 100, rounded % 100, a.Currency, a.TransactionID, a.Description);
        }

        // Перегрузка оператора деления на число
        public static Money operator /(Money a, double divisor)
        {
            if (divisor == 0)
                throw new DivideByZeroException("Деление на ноль недопустимо.");
            int total = a.Rubles * 100 + a.Kopeks;
            double result = total / divisor;
            int rounded = (int)Math.Round(result, MidpointRounding.AwayFromZero);
            return new Money(rounded / 100, rounded % 100, a.Currency, a.TransactionID, a.Description);
        }

        // Переопределение метода ToString для форматированного вывода суммы
        public override string ToString()
        {
            string sign = Rubles < 0 ? "-" : "";
            int absRubles = Math.Abs(Rubles);
            return $"{sign}{absRubles} руб. {Kopeks:D2} коп. ({Currency})";
        }
    }

    class Program
    {
        static void Main()
        {
            // Ввод исходной суммы
            Console.WriteLine("Введите исходную сумму денег:");
            Money money = InputMoney();

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nТекущая сумма: " + money);
                Console.WriteLine("\nВыберите операцию:");
                Console.WriteLine("1 - Рассчитать сумму перевода с комиссией");
                Console.WriteLine("2 - Прибавить другую сумму");
                Console.WriteLine("3 - Вычесть другую сумму");
                Console.WriteLine("4 - Умножить на коэффициент");
                Console.WriteLine("5 - Разделить на число");
                Console.WriteLine("0 - Выход");
                Console.Write("Опция: ");
                string option = Console.ReadLine();

                try
                {
                    switch (option)
                    {
                        case "1":
                            Console.Write("Введите величину комиссии в процентах: ");
                            double commission = ReadDouble();
                            Money transfer = money.TransferCost(commission);
                            Console.WriteLine($"Сумма с комиссией {commission}%: " + transfer);
                            // Если нужно, можно обновить money, например:
                            // money = transfer;
                            break;
                        case "2":
                            Console.WriteLine("Введите сумму для прибавления:");
                            Money moneyToAdd = InputMoney();
                            money = money + moneyToAdd;
                            Console.WriteLine("Новая сумма: " + money);
                            break;
                        case "3":
                            Console.WriteLine("Введите сумму для вычитания:");
                            Money moneyToSubtract = InputMoney();
                            money = money - moneyToSubtract;
                            Console.WriteLine("Новая сумма: " + money);
                            break;
                        case "4":
                            Console.Write("Введите коэффициент для умножения: ");
                            double multiplier = ReadDouble();
                            money = money * multiplier;
                            Console.WriteLine("Новая сумма: " + money);
                            break;
                        case "5":
                            Console.Write("Введите делитель для деления: ");
                            double divisor = ReadDouble();
                            if (divisor == 0)
                            {
                                Console.WriteLine("Деление на ноль недопустимо.");
                                break;
                            }
                            money = money / divisor;
                            Console.WriteLine("Новая сумма: " + money);
                            break;
                        case "0":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Неверная опция, попробуйте снова.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                }
            }

            Console.WriteLine("\nНажмите любую клавишу для завершения...");
            Console.ReadKey();
        }

        // Метод для ввода суммы от пользователя
        static Money InputMoney()
        {
            Console.Write("Рубли: ");
            int rubles = ReadInt();

            Console.Write("Копейки: ");
            int kopeks = ReadInt();

            Console.Write("Валюта (по умолчанию RUB): ");
            string currency = Console.ReadLine();
            if (string.IsNullOrEmpty(currency))
                currency = "RUB";

            Console.Write("ID транзакции (опционально): ");
            string transactionID = Console.ReadLine();

            Console.Write("Описание (опционально): ");
            string description = Console.ReadLine();

            Money money = new Money(rubles, kopeks, currency, transactionID, description);
            return money;
        }

        // Метод для безопасного чтения целого числа
        static int ReadInt()
        {
            int result;
            while (!int.TryParse(Console.ReadLine(), out result))
            {
                Console.Write("Неверный ввод. Введите целое число: ");
            }
            return result;
        }

        // Метод для безопасного чтения числа с плавающей запятой
        static double ReadDouble()
        {
            double result;
            while (!double.TryParse(Console.ReadLine(), out result))
            {
                Console.Write("Неверный ввод. Введите число: ");
            }
            return result;
        }
    }
}
