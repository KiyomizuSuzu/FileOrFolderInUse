<details><summary>日本語</summary>

# FileOrFolderInUse
C#で作った[WinForms](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/overview)ベースのGUIツール。[MicrosoftのSysinternals Handle](https://learn.microsoft.com/en-us/sysinternals/downloads/handle)を組み込んで、ファイルやフォルダを使用しているプロセスを特定し、そのまま強制終了できる。
## 右クリックメニューへの統合
以下のレジストリエントリを追加する：
- `HKEY_CLASSES_ROOT\*\shell\FileOrFolderInUse`：既定値（""）を `Show file handles` に設定
- `HKEY_CLASSES_ROOT\*\shell\FileOrFolderInUse\command`：既定値（""）を `"C:\Path\To\FileOrFolderInUse.exe" "%1"` に設定
- `HKEY_CLASSES_ROOT\Directory\shell\FileOrFolderInUse`：既定値（""）を `Show folder handles` に設定
- `HKEY_CLASSES_ROOT\Directory\shell\FileOrFolderInUse\command`：既定値（""）を `"C:\Path\To\FileOrFolderInUse.exe" "%1"` に設定
### 使い方
1. https://learn.microsoft.com/en-us/sysinternals/downloads/handle からHandle v5.0をダウンロードし、`handle.exe`・`handle64.exe`・`handle64a.exe` を `FileOrFolderInUse.exe` と同じフォルダに置く。
2. `FileOrFolderInUse.exe` を、調べたいファイルやフォルダを引数に指定して実行する：
```powershell
FileOrFolderInUse "C:\Path\To\File.txt" "C:\Path\To\Directory"
```
3. 該当プロセスを終了するかどうかをYes/Noで確認。<br><br><img src="UI.png" width="400" />
### 動作の流れ（フローチャート）
---
![Flowchart](FileOrFolderInUse.drawio.svg)

---
### ビルド方法
.NET 10 SDKが必要。https://dotnet.microsoft.com/ja-jp/download/dotnet/10.0 からインストールできる。

リポジトリのルートでPowerShellを開き、以下を実行するだけ。
```powershell
dotnet publish -c Release
```
## AGPL-3.0 ライセンス
参照：https://licenses.opensource.jp/AGPL-3.0/AGPL-3.0.html

[OSI承認済み](https://opensource.org/licenses?ls=GNU+Affero+General+Public+License+version+3)のオープンソースライセンス。AGPL-3.0の条件のもとで、自由にフォーク・改変・再配布してもらって構わない。

AGPL-3.0に従う以上、対象コードは同じライセンスのまま維持する必要があり、別のライセンスへの再ライセンスはできない。また、このソフトウェアを受け取った人（購入やサービス経由も含む）には、同じライセンス条件のもとで対応するソースコードへのアクセスを提供する必要がある。

ライセンス全文は[LICENSE.txt](https://github.com/KiyomizuSuzu/FileOrFolderInUse/blob/main/LICENSE.txt)を参照。
</details>

---
<details open><summary>English</summary>

# FileOrFolderInUse
This is a [WinForms](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/overview) Graphical User Interface made in C# that integrates the [Microsoft's Sysinternals Handle](https://learn.microsoft.com/en-us/sysinternals/downloads/handle) to identify processes using a file or directory, with the option to close them.
## Context Menu Integration
Add the following registry entries:
- `HKEY_CLASSES_ROOT\*\shell\FileOrFolderInUse` with Default ("") value set to `Show file handles`
- `HKEY_CLASSES_ROOT\*\shell\FileOrFolderInUse\command` with Default ("") value set to `"C:\Path\To\FileOrFolderInUse.exe" "%1"`
- `HKEY_CLASSES_ROOT\Directory\shell\FileOrFolderInUse` with Default ("") value set to `Show folder handles`
- `HKEY_CLASSES_ROOT\Directory\shell\FileOrFolderInUse\command` with Default ("") value set to `"C:\Path\To\FileOrFolderInUse.exe" "%1"`
### How to use
1. Download Handle v5.0 from https://learn.microsoft.com/en-us/sysinternals/downloads/handle and place `handle.exe`, `handle64.exe`, `handle64a.exe` in the same folder as `FileOrFolderInUse.exe`.
2. Run `FileOrFolderInUse.exe` with arguments to specify one or more (files or directories) for inspection:
```powershell
FileOrFolderInUse "C:\Path\To\File.txt" "C:\Path\To\Directory"
```
3. Confirm Yes or No to close these processes or not. <br><br><img src="UI.png" width="400" />
### How it works (Flowchart)
---
![Flowchart](FileOrFolderInUse.drawio.svg)

---
### To build the source code
Ensure you have .NET 10 SDK installed from https://dotnet.microsoft.com/en-us/download/dotnet/10.0

Then, open up Powershell in the repository root directory and run the following command:
```powershell
dotnet publish -c Release
```
## AGPL-3.0 license
Source: https://www.gnu.org/licenses/agpl-3.0.en.html

This is an [OSI-approved](https://opensource.org/licenses?ls=GNU+Affero+General+Public+License+version+3) open-source license. Free to fork, modify, and redistribute under the terms of the AGPL-3.0.

By complying with the AGPL-3.0 license, you must keep the same license for the covered work and cannot relicense that covered part under a different license.
Anyone who receives the software (including through purchase or as a service) must also be provided access to the corresponding source code under the same license.

See the [LICENSE.txt](https://github.com/KiyomizuSuzu/FileInUse/blob/main/LICENSE.txt) for the full license text.
</details>