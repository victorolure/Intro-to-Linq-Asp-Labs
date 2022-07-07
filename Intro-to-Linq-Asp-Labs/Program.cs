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
    Hotel.ReserveRoom(4, 4, testRoom, new DateTime(2022,7, 9));
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}



Hotel.AutomaticReservation(1, 5, new DateTime(2022, 7, 8));

Console.WriteLine("\nAfter auto-reservation of first room with capacity of 5, room 5 is thrown out \n");
foreach (Room room in Hotel.GetVacantRooms())
{
    Console.WriteLine($"Room number: {room.Number} Capacity: {room.Capacity}");
}

Hotel.AutomaticReservation(2, 5, new DateTime(2022, 7, 6));//First reservation for a date with client with client Id 2;

try
{
    Hotel.AutomaticReservation(2, 2, new DateTime(2022, 7, 6)); // Attempt to make second reservation for same date.
}
catch(Exception ex)
{
    Console.WriteLine(ex.Message);
}

Hotel.AutomaticReservation(3, 2, new DateTime(2022, 7, 11));
Hotel.AutomaticReservation(5, 5, new DateTime(2022, 7, 12));


Hotel.Checkin("Olu"); // After this checkin, Room capacity reduces to 35 from the initial 37

Console.WriteLine(Hotel.TotalCapacityRemaining());

Console.WriteLine(Hotel.AverageOccupancyPercentage());

foreach(Room room in Hotel.Rooms)
{
    if (room.Occupied)
    {
        Console.WriteLine(room.Reservations.First(r => r.Current == true).Client.Name); // To display the name of the client that checked in
    }
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

    public static Reservation ReserveRoom(int occupants, int clientId, Room room, DateTime startdate)
    {
        Client client = Hotel.GetClient(clientId);
        if (client != null)
        {
            if(client.Reservations.FirstOrDefault(r => r.StartDate.Date == startdate.Date)== null)
            {
                if (occupants <= room.Capacity && room.Occupied == false)
                {
                    Reservation newReservation = new Reservation(occupants, client, room, startdate);
                    newReservation.Id = Reservations.Count + 1;
                    Reservations.Add(newReservation);
                    room.Reservations.Add(newReservation);
                    client.Reservations.Add(newReservation);
                    return newReservation;
                }
                else
                {
                    throw new Exception("Error: No room available with this capacity");
                }
            }
            else
            {
                throw new Exception($"Error: Sorry {client.Name}, you cannot make 2 reservations for same start date");
            }
            

        }
        else
        {
            throw new Exception("Error: No Registered Client with this Id, Please get registered");
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

    public static Reservation AutomaticReservation(int clientId, int occupants, DateTime startdate)
    {
        Client client = GetClient(clientId);
        Room autoRoom = Rooms.FirstOrDefault(r => !r.Occupied && r.Capacity >= occupants);

        if(client != null)
        {
            if (client.Reservations.FirstOrDefault(r => r.StartDate.Date == startdate.Date) == null)
            {
                if (autoRoom != null)
                {
                    Reservation autoReservation = new Reservation(occupants, client, autoRoom, startdate);
                    autoReservation.Id = Reservations.Count + 1;
                    client.Reservations.Add(autoReservation);
                    autoRoom.Reservations.Add(autoReservation);
                    Reservations.Add(autoReservation);
                    return autoReservation;
                }
                else
                {
                    throw new Exception("Error: No Room available with this capacity");
                }
            }
            else
            {
                throw new Exception($"Error: Sorry, {client.Name}, you cannot make 2 reservations for same start date");
            }

             
        }
        else
        {
            throw new Exception("No Client with this ID");
        }
       
        
    }

    public static void Checkin(string clientName)
    {
        Client client = Clients.FirstOrDefault(c => c.Name.ToUpper() == clientName.ToUpper());
        Reservation reservationToUse;
        if (client != null)
        {
           reservationToUse = client.Reservations.FirstOrDefault(r => r.StartDate.Date == DateTime.Now.Date);
            if(reservationToUse != null)
            {
                reservationToUse.Current = true;
                reservationToUse.Room.Occupied = true;
            }
        }
    }

    public static void CheckoutRoom(int clientId)
    {
        Client client = GetClient(clientId);
        Reservation reservationToCheckOut;
        if (client != null)
        {
            reservationToCheckOut = client.Reservations.FirstOrDefault(r => r.Current == true);
            reservationToCheckOut.Current = false;
            reservationToCheckOut.Room.Occupied = false;
        }
    }

    public static void CheckoutRoom(string clientName)
    {
        Client client = Clients.FirstOrDefault(c => c.Name.ToUpper() == clientName.ToUpper());
        Reservation reservationToCheckOut;
        if(client != null)
        {
           reservationToCheckOut = client.Reservations.FirstOrDefault(r=> r.Current == true);
           reservationToCheckOut.Current = false;
           reservationToCheckOut.Room.Occupied = false;
        }
    }


    //public static int TotalCapacityRemaining()
    //{
    //    return Rooms.Aggregate(0, (current, r) => current + r.Capacity - r.Reservations.Aggregate(0,(current, r)=>current+ r.Occupants));
    //}

    public static int TotalCapacityRemaining()
    {
       int totalCapacity = 0;
        foreach(Room r in Rooms)
        {
            totalCapacity+= r.Capacity - GetTotalOccupants(r);
        }
        return totalCapacity;
    }

    public static int GetTotalOccupants(Room room)//helper function for TotalCapacityRemaining();
    {
        int totalOccupants = 0;
        foreach(Reservation r in room.Reservations)
        {
            if(r.Current == true)
            {
                totalOccupants += r.Occupants;
            }
        }
        return totalOccupants;
    }

    public static int AverageOccupancyPercentage()
    {
        int totalAverage = 0;
        foreach (Room room in Rooms)
        {
            if(room.Occupied == true)
            {
                totalAverage += GetTotalOccupants(room) / room.Capacity;
            }
        }
        return totalAverage;
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
    public DateTime Created { get; set; }
    public DateTime StartDate { get; set; }
    public int Id { get; set; }
    public int Occupants { get; set; }
    public bool Current { get; set; }
    public Client Client { get; set; }
    public Room Room { get; set; }


    // CONSTRUCTORS
    public Reservation() { }
    public Reservation(int occupants, Client client, Room room, DateTime startdate)
    {
        Created = DateTime.Now;
        StartDate = startdate;
        Occupants = occupants;
        Current = false;
        Client = client;
        Room = room;
    }
}