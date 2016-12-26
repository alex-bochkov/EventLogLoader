
Module Module1

    Sub Main()

        Dim Service = New EventLogLoaderService.EventLogLoaderService
        Service.SubStart()
        Console.WriteLine("Service has been started")
        Console.ReadKey()

        Service.SubStop()
        Console.WriteLine("Service has been stopped")

    End Sub

End Module
