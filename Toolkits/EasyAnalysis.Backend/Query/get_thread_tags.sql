SELECT [Tags].[Name]
FROM [uwpdb].[dbo].[ThreadTags] [ThreadTags]
  INNER JOIN [uwpdb].[dbo].[Tags] [Tags]
  ON [ThreadTags].[TagId] = [Tags].[Id]
WHERE [ThreadId] = @ThreadId