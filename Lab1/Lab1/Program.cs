using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;

[XmlInclude(typeof(Residential))]
[XmlInclude(typeof(NonResidential))]
[DataContract]
abstract public class Immovables
{
    [DataMember]
    public double quantity;
    [DataMember]
    public string address;
    [DataMember]
    public string type;
    public abstract void AverageAmount();
}
class AmountComparer : IComparer<Immovables>
{
    public int Compare(Immovables i1, Immovables i2)
    {

        if (i1.quantity > i2.quantity)
        {
            return -1;
        }
        else if (i1.quantity == i2.quantity)
        {
            if (i1.type.Length < i2.type.Length)
            {
                return -1;
            }
            else if (i1.type.Length == i2.type.Length)
            {
                if (i1.address[0] > i2.address[0])
                {
                    return 1;
                }
                else if (i1.address[0] < i2.address[0])
                    return -1;
            }
            else if (i1.type.Length > i2.type.Length)
            {
                return 1;
            }
        }
        else if (i1.quantity < i2.quantity)
        {
            return 1;
        }

        return 0;
    }
}
[KnownType(typeof(Immovables))]
[KnownType(typeof(Residential))]
[KnownType(typeof(NonResidential))]
[DataContract]
public class ManagementCompany
{
    public List<Immovables> list = new List<Immovables>();
    double avgQuantity;
    public ManagementCompany() { }
    public int Add(Immovables building)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].address == building.address)
            {
                return 1;
            }
        }
        list.Add(building);
        AverageAmount();
        return 0;
    }
    public void AverageAmount()
    {
        double sum = 0;
        for (int i = 0; i < list.Count; i++)
        {
            sum += list[i].quantity;
        }
        avgQuantity = sum / list.Count;
    }
    public void Del(Immovables building)
    {
        list.Remove(building);
    }
    public void Sort()
    {
        AmountComparer ac = new AmountComparer();
        list.Sort(ac);
    }
    public void xmlOutput(string fileName)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(ManagementCompany));
        TextWriter writer = new StreamWriter(fileName);
        try { serializer.Serialize(writer, this); }
        catch (SerializationException e)
        {
            Console.WriteLine("Output Error");
            throw e;
        }
        writer.Close();
    }

    public void xmlInput(string fileName)
    {
        try
        {
            TextReader reader = new StreamReader(fileName);
            XmlSerializer serializer = new XmlSerializer(typeof(ManagementCompany));
            ManagementCompany newCompany = (ManagementCompany)serializer.Deserialize(reader);
            reader.Close();

            int size = newCompany.list.Count;
            for (int i = 0; i < size; i++)
                Add(newCompany.list[i]);
            AverageAmount();
        }
        catch (SerializationException e)
        {
            Console.WriteLine("Input Error");
            throw e;
        }
    }
    public void jsonOutput(string fileName)
    {
        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ManagementCompany));
        StreamWriter w = new StreamWriter(fileName);
        try { serializer.WriteObject(w.BaseStream, this); }
        catch (SerializationException e)
        {
            Console.WriteLine("Output Error");
            throw e;
        }
        w.Close();
    }

    public void jsonInput(string fileName)
    {
        try
        {
            StreamReader reader = new StreamReader(fileName);
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ManagementCompany));
            ManagementCompany newCompany = (ManagementCompany)serializer.ReadObject(reader.BaseStream);
            int size = newCompany.list.Count;
            for (int i = 0; i < size; i++)
                Add(newCompany.list[i]);
            AverageAmount();
            reader.Close();
        }
        catch (SerializationException e)
        {
            Console.WriteLine("Input Error");
            throw e;
        }
    }

}

[DataContract]
public class Residential : Immovables
{
    double QuantityOfApartments;
    double QuantityOfRooms;
    public Residential(double QuantityOfApartments, double QuantityOfRooms, string address)
    {
        this.address = address;
        this.QuantityOfApartments = QuantityOfApartments;
        this.QuantityOfRooms = QuantityOfRooms;
        this.type = "residential";
        AverageAmount();
    }

    public override void AverageAmount()
    {
        this.quantity = QuantityOfApartments * QuantityOfRooms * 1.3;

    }
}

[DataContract]
public class NonResidential : Immovables
{
    int square;
    public NonResidential(int square, string address)
    {
        this.address = address;
        this.square = square;
        this.type = "NonResidential";
        AverageAmount();
    }

    public override void AverageAmount()
    {
        this.quantity = square * 0.2;
    }
}
public class Menu
{
    ManagementCompany mc = new ManagementCompany();
    public void Sort(ManagementCompany mc)
    {
        mc.Sort();
    }
    public void AllOutput(ManagementCompany mc)
    {
        for (int i = 0; i < mc.list.Count; i++)
        {
            Console.Write(mc.list[i].type);
            Console.Write(" ");
            Console.Write(mc.list[i].address);
            Console.Write(" ");
            Console.WriteLine(mc.list[i].quantity);

        }
    }
    public void Top3(ManagementCompany mc)
    {
        for (int i = 0; i < 3; i++)
        {
            Console.Write(mc.list[i].type);
            Console.Write(" ");
            Console.Write(mc.list[i].address);
            Console.Write(" ");
            Console.WriteLine(mc.list[i].quantity);
        }
    }
    public void Last4(ManagementCompany mc)
    {
        for (int i = mc.list.Count - 4; i < mc.list.Count; i++)
        {
            Console.WriteLine(mc.list[i].address);
        }
    }
    public void XmlOutput(ManagementCompany mc)
    {
        string filename = Console.ReadLine();
        mc.xmlOutput(filename);
    }
    public void JsonOutput(ManagementCompany mc)
    {
        string filename = Console.ReadLine();
        mc.jsonOutput(filename);
    }
    public void XmlInput(ManagementCompany mc)
    {
        string filename = Console.ReadLine();
        mc.xmlInput(filename);
    }
    public void JsonInput(ManagementCompany mc)
    {
        string filename = Console.ReadLine();
        mc.jsonInput(filename);
    }
}
class Program
{
    static void Main(string[] args)
    {
        //Residential r1 = new Residential(10, 1, "cddress1");
        //NonResidential nr1 = new NonResidential(65, "address2");
        //Residential r2 = new Residential(67, 2, "address3");
        //NonResidential nr2 = new NonResidential(240, "address4");
        //Residential r3 = new Residential(10, 1, "address5");
        //NonResidential nr3 = new NonResidential(360, "address6");
        //Residential r4 = new Residential(10, 1, "bddress7");
        //NonResidential nr4 = new NonResidential(480, "address8");
        //Residential r5 = new Residential(35, 4, "address9");
        //NonResidential nr5 = new NonResidential(600, "address10");

        ManagementCompany mc = new ManagementCompany();

        //mc.Add(r1);
        //mc.Add(nr1);
        //mc.Add(r2);
        //mc.Add(nr2);
        //mc.Add(r3);
        //mc.Add(nr3);
        //mc.Add(r4);
        //mc.Add(nr4);
        //mc.Add(r5);
        //mc.Add(nr5);

        Menu m = new Menu();
        while (true)
        {
            Console.WriteLine("1) - Sort");
            Console.WriteLine("2) - Output all items");
            Console.WriteLine("3) - Conclusion of the first 3 elements");
            Console.WriteLine("4) - Displays the last 4 addresses");
            Console.WriteLine("5) - Outout xml");
            Console.WriteLine("6) - Input xml");
            Console.WriteLine("7) - Output json");
            Console.WriteLine("8) - Input json");
            Console.WriteLine("9) - Exit");
            int choice = int.Parse(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    m.Sort(mc);
                    break;
                case 2:
                    m.AllOutput(mc);
                    break;
                case 3:

                    m.Top3(mc);
                    break;
                case 4:
                    m.Last4(mc);
                    break;
                case 5:
                    m.XmlOutput(mc);
                    break;
                case 6:
                    m.XmlInput(mc);
                    break;
                case 7:
                    m.JsonOutput(mc);
                    break;
                case 8:
                    m.JsonInput(mc);
                    break;
                case 9:
                    return;

            }
        }
    }
}
