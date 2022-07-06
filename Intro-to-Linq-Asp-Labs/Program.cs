Hotel.RegisterClient("Vic", 1111222244446666);
Hotel.RegisterClient("Olu", 1234321212332322);
Hotel.RegisterClient("Kay", 1234443344667788);
Hotel.RegisterClient("Jae", 2222444455556666);
Hotel.RegisterClient("kim", 0000999988887777);
Hotel.CreateRoom(4);
Hotel.CreateRoom(4);
Hotel.CreateRoom(4);
Hotel.CreateRoom(4);
Hotel.CreateRoom(5);
Hotel.CreateRoom(2);
Hotel.CreateRoom(2);
Hotel.CreateRoom(2);
Hotel.CreateRoom(5);
Hotel.CreateRoom(5);



Room testRoom = new Room(5);


Console.WriteLine("Initial Vacant rooms\n");
foreach (Room room in Hotel.GetVacantRooms())
{
    Console.WriteLine($"Room number: {room.Number} Capacity: {room.Capacity}");
}


try
{
    Hotel.ReserveRoom(4, 4, testRoom);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

Hotel.AutomaticReservation(1, 5);

Console.WriteLine("\nAfter auto-reservation of first room with capacity of 5, room 5 is thrown out \n");
foreach (Room room in Hotel.GetVacantRooms())
{
    Console.WriteLine($"Room number: {room.Number} Capacity: {room.Capacity}");
}

Hotel.AutomaticReservation(2, 2);
Hotel.AutomaticReservation(2, 2);
Hotel.AutomaticReservation(3, 2);
Hotel.AutomaticReservation(5, 5);

Console.WriteLine("\nTop 3 Clients with highest reservations\n");
foreach (Client client in Hotel.TopThreeClients())
{
    Console.WriteLine($"Client Name: {client.Name}");
}

Console.WriteLine("\nRemaining vacant rooms after multiple auto reservations\n");
foreach (Room room in Hotel.GetVacantRooms())
{
    Console.WriteLine($"Room number: {room.Number} Capacity: {room.Capacity}");
}






static class Hotel
{
    public static string Name { get; set; } = "EleventhHouse";
    public static string Address { get; set; } = "130 Henlow-Bay";
    public static List<Client> Clients { get; set; } = new List<Client>();
    public static int NumberOfClients { get; set; } = 0;
    public static List<Room> Rooms { get; set; } = new List<Room>();
    public static List<Reservation> Reservations { get; set; } = new List<Reservation>();


    public static Client RegisterClient(string name, long creditcard)
    {
        Client newClient = new Client(name, creditcard);
        newClient.Id = Clients.Count + 1;
        Clients.Add(newClient);
        NumberOfClients++;
        return newClient;

    }

    public static Room CreateRoom(int capacity)
    {
        Room newroom = new Room(capacity);
        newroom.Number = Rooms.Count + 1;
        Rooms.Add(newroom);
        return newroom;
    }

    public static void RemoveClient(int id)
    {
        Client client = Clients.First(client => client.Id == id);
        Clients.Remove(client);
    }

    public static Client GetClient(int clientId)
    {
        Client searchedClient = Hotel.Clients.FirstOrDefault(client => client.Id == clientId);
        return searchedClient;
    }

    public static Reservation ReserveRoom(int occupants, int clientId, Room room)
    {
        Client client = Hotel.GetClient(clientId);
        if (client != null)
        {
            if (occupants <= room.Capacity && room.Occupied == false)
            {
                Reservation newReservation = new Reservation(occupants, client, room);
                room.Occupied = true;
                newReservation.Id = Reservations.Count + 1;
                Reservations.Add(newReservation);
                room.Reservations.Add(newReservation);
                client.Reservations.Add(newReservation);
                return newReservation;
            }
            else
            {
                throw new Exception("Error: No available room with this capacity");
            }

        }
        else
        {
            throw new Exception("Error: No Registered Client with this Id");
        }
    }

    public static Reservation GetReservation(int id)
    {
        Reservation reservationsearch = Reservations.First(r => r.Id == id);
        return reservationsearch;
    }

    public static List<Room> GetVacantRooms()
    {
        List<Room> vacantrooms = Rooms.Where(r => !r.Occupied).ToList();
        return vacantrooms;
    }

    public static List<Client> TopThreeClients()
    {
        List<Client> topThreeClients = Clients.OrderByDescending(client => client.Reservations.Count).Take(3).ToList();
        return topThreeClients;
    }

    public static Reservation AutomaticReservation(int clientId, int occupants)
    {
        Client client = Clients.First(c => c.Id == clientId);
        Room autoRoom = Rooms.First(r => !r.Occupied && r.Capacity >= occupants);
        Reservation autoReservation = new Reservation(occupants, client, autoRoom);
        autoReservation.Id = Reservations.Count + 1;
        client.Reservations.Add(autoReservation);
        autoRoom.Reservations.Add(autoReservation);
        autoRoom.Occupied = true;
        Reservations.Add(autoReservation);
        return autoReservation;
    }


}

class Room
{
    public int Number { get; set; }
    public int Capacity { get; set; }
    public bool Occupied { get; set; }
    public List<Reservation> Reservations { get; set; }

    public Room()
    {
        Reservations = new List<Reservation>();
    }

    public Room(int capacity)
    {

        Capacity = capacity;
        Occupied = false;
        Reservations = new List<Reservation>();
    }

}




class Client
{
    public string Name { get; set; }
    public int Id { get; set; }
    public long CreditCard { get; set; }
    public List<Reservation> Reservations { get; set; }

    public Client(string name, long creditCard)
    {
        Name = name;
        CreditCard = creditCard;
        Reservations = new List<Reservation>();
    }
}



class Reservation
{
    public DateTime Date { get; set; }
    public int Id { get; set; }
    public int Occupants { get; set; }
    public bool IsCurrent { get; set; }
    public Client Client { get; set; }
    public Room Room { get; set; }


    // CONSTRUCTORS
    public Reservation() { }
    public Reservation(int occupants, Client client, Room room)
    {
        Date = DateTime.Now;
        Occupants = occupants;
        IsCurrent = true;
        Client = client;
        Room = room;
    }
}