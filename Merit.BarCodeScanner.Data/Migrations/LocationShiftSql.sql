IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[V_LocationShifts]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[V_LocationShifts]
AS
select l.Id as LocationId,ls.Id as ShiftId,ls.Start as [Start], ls.[End] as [End],ls.SpansDays as SpansDays from MeritProductionTest.dbo.LocationShift ls 
join MeritProductionTest.dbo.Location l on ls.LocationId = l.Id'