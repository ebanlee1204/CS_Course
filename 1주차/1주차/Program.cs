namespace _1주차
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter your height and weight: ");
            string input = Console.ReadLine();

            string[] height_weight = input.Split(' ');

            float height = float.Parse(height_weight[0]) / 100;
            float weight = float.Parse(height_weight[1]);  

            Console.WriteLine("Your BMI is: {0}", weight / (height * height));
        }
    }
}