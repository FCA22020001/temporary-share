# Script

```
Family --
        | - Child (main executable script)
        | - Support (Data of Child)
```

- 処理
    - Family
        - 必要とされるスクリプトを読み込む
        - Childを呼び出す
    - Child
        - Supportを使って処理を行う
    - Support
        - Childから呼び出される
