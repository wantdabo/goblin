# 生成代码
mpc -i "..\Queen.Protocols\Queen.Protocols.csproj" -o "..\Queen.Protocols\Common\MessagePackGenerated.cs" -m

$filePath = "..\Queen.Protocols\Common\MessagePackGenerated.cs"
$firstLine = "#if GOBLIN"
$endLine = "#endif"

# 读取文件内容
$content = Get-Content -Path $filePath

# 在第一行添加 $firstLine
$newContent = @($firstLine) + $content

# 将修改后的内容写回文件
$newContent | Set-Content -Path $filePath

# 添加 $endLine 到文件的末尾
Add-Content -Path $filePath -Value $endLine

