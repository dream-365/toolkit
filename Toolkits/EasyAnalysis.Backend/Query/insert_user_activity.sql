INSERT INTO [dbo].[UserActivities]
           ([Hash]
           ,[UserId]
           ,[Action]
           ,[Time]
           ,[EffectOn])
     VALUES
           (@Hash
           ,@UserId
           ,@Action
           ,@Time
           ,@EffectOn)