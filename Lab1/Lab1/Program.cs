using System;
using System.Collections.Generic;

abstract public class Immovables
{
    public string name;
    public int quantity;
    public string address;
    public abstract double AverageAmount();
}

public class ManagementCompany
{
    public List<Immovables> list = new List<Immovables>();

}

public class Residential : Immovables
{
    public Residential() { }
    public Residential(int NumbeOfApartments, int NumbeOfRooms, string address)
    {
        this.address = address;
        this.NumbeOfApartments = NumbeOfApartments;
        this.NumbeOfRooms = NumbeOfRooms;
    }
    double NumberOfTenants;
    int NumbeOfApartments;
    int NumbeOfRooms;
    public override double AverageAmount()
    {
        NumberOfTenants = NumbeOfApartments * NumbeOfRooms * 1.3;
        return NumberOfTenants;
    }
}

public class NonResidential : Immovables
{
    public NonResidential(int square, string address)
    {
        this.address = address;
        this.square = square;
    }
    double NumberOfWorkers;
    int square;
    public override double AverageAmount()
    {
        NumberOfWorkers = square * 0.2;
        return NumberOfWorkers;
    }
}
class Program
{
    static void Main(string[] args)
    {
        Residential r = new Residential();
        Console.WriteLine("Select type immovables");
        Console.WriteLine("1) residential");
        Console.WriteLine("2) non-residential");
        int choose = int.Parse(Console.ReadLine());
        Console.WriteLine("Enter the address in the format st. number");
        string address = Console.ReadLine();

        switch (choose)
        {
            case 1:
                Console.WriteLine("Enter the number of apartments and rooms in them");
                int NumbeOfApartments = int.Parse(Console.ReadLine()), NumbeOfRooms = int.Parse(Console.ReadLine());
                r = new Residential(NumbeOfApartments, NumbeOfRooms, address);
                break;
            case 2:
                break;
            default:
                break;
        }
        Console.WriteLine(r.AverageAmount());
        Console.ReadKey();
    }
}
