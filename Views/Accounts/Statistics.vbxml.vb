Imports System.Text
Imports ChartJSCore.Models
Imports ChartJSCore.Helpers

Imports SPCMS.Models

Partial Public Class StatisticsView

    Public Overrides Function GetVbXml() As XElement
        Dim df As (Users As List(Of User), Spares As List(Of SpareParts), Orders As List(Of Order)) = ViewData!dataset
        Dim dataset As (Users As List(Of User), Spares As List(Of SpareParts), Orders As List(Of Order)) = (df.Users, df.Spares, df.Orders)

        Dim totalSales As Single = (From o As Order In dataset.Orders Join s As SpareParts In dataset.Spares On o.ID Equals s.ID Select (o.Quantity, s.Price)).Sum(Function(s) s.Price * s.Quantity),
            totalSold As Long = dataset.Orders.Count
        Return _
            <vbxml>
                <div class="row">
                    <div class="col">
                        <div><h5 class="fw-light">Percentage of registered users by sales</h5></div>
                        <div>
                            <canvas id="usersBySales"></canvas>
                        </div>
                    </div>
                    <div class="col">
                        <div class="card">
                            <div class="card-content">
                                <div class="card-body">
                                    <div class="media d-flex">
                                        <div class="align-self-center">
                                            <span class="iconify dashboard" data-icon="icon-park-outline:sales-report"></span>
                                        </div>
                                        <div class="media-body text-right">
                                            <h3>KSh <%= Format(totalSales, "#,#.00") %></h3>
                                            <span>Total Sales</span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col">
                        <div class="card">
                            <div class="card-content">
                                <div class="card-body">
                                    <div class="media d-flex">
                                        <div class="align-self-center">
                                            <span class="iconify dashboard" data-icon="ic:baseline-point-of-sale"></span>
                                        </div>
                                        <div class="media-body text-right">
                                            <h3><%= totalSold& %></h3>
                                            <span>Total Sold</span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col">
                        <div><h5 class="fw-light">Top 10 customers by purchases</h5></div>
                        <div>
                            <canvas id="top5"></canvas>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col">
                        <div><h5 class="fw-light">Best sellers</h5></div>
                        <div>
                            <canvas id="bestSeller"></canvas>
                        </div>
                    </div>
                    <div class="col">
                        <div class="card">
                            <div class="card-content">
                                <div class="card-body">
                                    <div class="media d-flex">
                                        <div class="align-self-center">
                                            <span class="iconify dashboard" data-icon="carbon:chart-average"></span>
                                        </div>
                                        <div class="media-body text-right">
                                            <h3>KSh <%= Format(If(totalSales = 0 OrElse totalSold = 0, 0, (totalSales / totalSold&)), "#,#.00") %></h3>
                                            <span>Average Sales</span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col">
                        <div><h5 class="fw-light">Selling products</h5></div>
                        <div>
                            <canvas id="Sellers"></canvas>
                        </div>
                    </div>
                </div>
                <environment include="Development">
                    <script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/3.9.1/chart.js" integrity="sha512-d6nObkPJgV791iTGuBoVC9Aa2iecqzJRE0Jiqvk85BhLHAPhWqkuBiQb1xz2jvuHNqHLYoN3ymPfpiB1o+Zgpw==" crossorigin="anonymous" referrerpolicy="no-referrer"></script>
                </environment>
                <environment exclude="Development">
                    <script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/3.9.1/chart.min.js" integrity="sha512-ElRFoEQdI5Ht6kZvyzXhYG9NqjtkmlkfYk0wr6wHxU9JEHakS7UJZNeml5ALk+8IKlU6jDgMabC3vkumRokgJA==" crossorigin="anonymous" referrerpolicy="no-referrer"></script>
                </environment>

                <script>
                    <%= (Function() As String
                             Dim js As New StringBuilder(vbLf)
                             Dim chrt As New Chart() With {
                                 .Type = Enums.ChartType.Pie,
                                 .Options = New Options With {
                                     .Responsive = True,
                                     .MaintainAspectRatio = False
                                 }
                             }
                             Dim data As New Data() With {
                                 .Labels = New List(Of String) From {"Users with purchases", "Users without purchases"}
                             }
                             Dim groupedWPurchase = dataset.Users.GroupJoin(dataset.Orders,
                                                                            Function(u As User) u.UserName,
                                                                            Function(o As Order) o.Username,
                                                                            Function(u, o) New With {.User = u, .Order = o})
                             Dim wPurchase = From u As User In dataset.Users Join o As Order In dataset.Orders On u.UserName Equals o.Username

                             Dim pieDataset As New PieDataset() With {
                                 .Label = "Registered users by sales",
                                 .Data = New List(Of Double?) From {wPurchase.Count, dataset.Users.Count - wPurchase.Count},
                                 .BackgroundColor = New List(Of ChartColor) From {ChartColor.FromHexString("#FF6384"), ChartColor.FromHexString("#36A2EB")},
                                 .BorderColor = New List(Of ChartColor) From {ChartColor.FromRgb(75, 192, 192)},
                                 .Circumference = .Circumference * 0.5
                             }
                             data.Datasets = New List(Of Dataset) From {pieDataset}
                             chrt.Data = data
                             js.Append(vbLf & chrt.CreateChartCode("usersBySales"))


                             chrt = New Chart() With {
                                 .Type = Enums.ChartType.Line,
                                 .Options = New Options With {
                                     .Responsive = True,
                                     .MaintainAspectRatio = False
                                 }
                             }
                             data = New Data() With {
                                 .Labels = groupedWPurchase.Select(Function(s) s.User.UserName).Take(10).ToList()
                             }
                             Dim lineDataset As New LineDataset() With {
                                 .Label = "Purchases",
                                 .Data = groupedWPurchase.Select(Function(c) New Double?(c.Order.Sum(Function(s) s.Quantity))).Take(10).ToList(),
                                 .Fill = "false",
                                 .Tension = 0.1#,
                                 .BackgroundColor = New List(Of ChartColor) From {ChartColor.FromRgba(75, 192, 192, 0.4)},
                                 .BorderColor = New List(Of ChartColor) From {ChartColor.FromRgb(75, 192, 192)},
                                 .BorderCapStyle = "butt",
                                 .BorderDash = New List(Of Integer) From {},
                                 .BorderDashOffset = 0.0#,
                                 .BorderJoinStyle = "miter",
                                 .PointBorderColor = New List(Of ChartColor) From {ChartColor.FromRgb(75, 192, 192)},
                                 .PointBackgroundColor = New List(Of ChartColor) From {ChartColor.FromHexString("#ffffff")},
                                 .PointBorderWidth = New List(Of Integer) From {1},
                                 .PointHoverRadius = New List(Of Integer) From {5},
                                 .PointHoverBackgroundColor = New List(Of ChartColor) From {ChartColor.FromRgb(75, 192, 192)},
                                 .PointHoverBorderColor = New List(Of ChartColor) From {ChartColor.FromRgb(220, 220, 220)},
                                 .PointHoverBorderWidth = New List(Of Integer) From {2},
                                 .PointRadius = New List(Of Integer) From {1},
                                 .PointHitRadius = New List(Of Integer) From {10},
                                 .SpanGaps = False
                             }
                             data.Datasets = New List(Of Dataset) From {lineDataset}
                             chrt.Data = data
                             js.Append(vbLf & chrt.CreateChartCode("top5"))


                             Dim groupedBListing = dataset.Orders.GroupBy(Function(g) g.Name).Select(Function(s) New With {
                                 s.First.Name, .TotalSales = s.Sum(Function(t) Val(t.Quantity))
                             })
                             chrt = New Chart() With {
                                 .Type = Enums.ChartType.Pie,
                                 .Options = New Options With {
                                     .Responsive = True,
                                     .MaintainAspectRatio = False
                                 }
                             }
                             data = New Data() With {
                                 .Labels = groupedBListing.Select(Function(s) s.Name).Take(10).ToList()
                             }
                             pieDataset = New PieDataset() With {
                                 .Label = "Purchases",
                                 .Data = groupedBListing.Select(Function(s) New Double?(s.TotalSales)).Take(10).ToList(),
                                 .BackgroundColor = New List(Of ChartColor) From {ChartColor.FromHexString("#FF6384"), ChartColor.FromHexString("#36A2EB")},
                                 .BorderColor = New List(Of ChartColor) From {ChartColor.FromRgb(75, 192, 192)},
                                 .Circumference = .Circumference * 0.5
                             }
                             data.Datasets = New List(Of Dataset) From {pieDataset}
                             chrt.Data = data
                             js.Append(vbLf & chrt.CreateChartCode("bestSeller"))


                             groupedBListing = groupedBListing.OrderBy(Function(o) o.TotalSales)
                             chrt = New Chart() With {
                                 .Type = Enums.ChartType.Bar,
                                 .Options = New Options With {
                                     .Responsive = True,
                                     .MaintainAspectRatio = False
                                 }
                             }
                             data = New Data() With {
                                 .Labels = groupedBListing.Select(Function(s) s.Name).Take(10).ToList()
                             }
                             Dim barDataset As New BarDataset() With
                             {
                                  .Label = "Products sold",
                                  .Data = groupedBListing.Select(Function(s) New Double?(s.TotalSales)).Take(10).ToList(),
                                  .BackgroundColor = New List(Of ChartColor) From
                                  {
                                      ChartColor.FromRgba(255, 99, 132, 0.2),
                                      ChartColor.FromRgba(54, 162, 235, 0.2),
                                      ChartColor.FromRgba(255, 206, 86, 0.2),
                                      ChartColor.FromRgba(75, 192, 192, 0.2),
                                      ChartColor.FromRgba(153, 102, 255, 0.2),
                                      ChartColor.FromRgba(255, 159, 64, 0.2)
                                  },
                                  .BorderColor = New List(Of ChartColor) From
                                  {
                                      ChartColor.FromRgb(255, 99, 132),
                                      ChartColor.FromRgb(54, 162, 235),
                                      ChartColor.FromRgb(255, 206, 86),
                                      ChartColor.FromRgb(75, 192, 192),
                                      ChartColor.FromRgb(153, 102, 255),
                                      ChartColor.FromRgb(255, 159, 64)
                                  },
                                  .BorderWidth = New List(Of Integer) From {1}
                             }
                             data.Datasets = New List(Of Dataset) From {barDataset}
                             chrt.Data = data
                             js.Append(vbLf & chrt.CreateChartCode("Sellers"))

                             Return js.ToString().Replace("var", "let")
                         End Function)() %>

                         $(document).ready(function () {
                             init_session("<%= Session.User %>", "<%= CInt(Session.Role) %>");
                         });
                </script>
                <style>
                    .card {
                        top: 50%;
                        -ms-transform: translateY(-50%);
                        transform: translateY(-50%);
                    }
                    
                    .dashboard {
                        font-size: 3em;
                        margin-right: 2vw;
                    }
                </style>
            </vbxml>

    End Function

End Class
