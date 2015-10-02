
Module Module1

    Sub Main()

        Dim Service = New EventLogLoaderService.EventLogLoaderService
        Service.SubStart()
        Console.WriteLine("Служба запущена")
        Console.ReadKey()

        Service.SubStop()
        Console.WriteLine("Служба остановлена")

    End Sub

End Module
