> [English - Translated by Google](https://translate.googleusercontent.com/translate_c?act=url&depth=1&hl=ja&ie=UTF8&prev=_t&rurl=translate.google.co.jp&sl=ja&sp=nmt1&tl=en&u=https://github.com/NNNIC/slag&usg=ALkJrhg0cc2ax8-HdmuZB7xKGm1U0a_pbA)

# slag (Something Like A Gei-es[JS])

Unity用組込スクリプトシステム

## 概要

Unityアプリケーション上で実行できるスクリプトシステムを提供

<a href="https://raw.githubusercontent.com/NNNIC/wiki_depot/master/slag/images/readme.png"><img src="https://github.com/NNNIC/wiki_depot/blob/master/slag/images/readme.png" width=480px /></a>

## 特徴

* アプリケーションに組込みロード・実行
* Unityクラス・.NETクラスおよびアプケーションクラスへシームレスに参照・変更・実行が可能
* Javascriptライクな文法のため学習コストが少なく、Javascript対応のエディタを使うことが可能
* .NETの機能に依存しない解析エンジン・ランタイムのためマルチプラットフォームで実行可能
* コンパイル時間を省くためバイナリデータからの実行が可能  
* 簡易デバッガ提供(Windowsのみ) 
* チェックサムによる改竄防止

## 用途

スクリプトとアプリの連携を計画的に行うことで柔軟なシステムを構築することが出来る。  
開発の各フェーズで効果が発揮でき、特に開発終盤の調整段階で多大な貢献が期待できる。

### 1. デバッガとして利用  

C#のクラスとシームレスにアクセスできるので、例えばホットキーでデバッグスクリーンを開きスクリプトで状況の確認が可能。<br>

### 2. テストとして利用

アプリが巨大化するとコンパイル時間が長くなり、修正のスピードが落ちる。<br>
実行バイナリを変更する必要がないスクリプトなら即実行ができ、フィードバックを早めることが出来る。<br>

### 3．ツールとして使用

スクリプトでツールコードを書くことで、実行バイナリを変更せずにツールとして使うことが可能。<br>
製品に限りなく近いため、品質とパフォーマンスを同時に確認することが出来る。  <br>

#### 利用例  

* メニューエディタ  
* カットシーン   
* レベルエディタ

### 4. テストの自動化に利用

スクリプトで操作テストをくみ上げれば、テストの自動化へ。<br>

### 5. プログラムの一部をスクリプト実行

仕様変更が多く柔軟性が求められる部分をスクリプト前提で実装。<br>

#### 例

* メニュー


### 6．アプリ配布後に動作またはデータを柔軟に変更

サーバ上に用意したスクリプトを読込ませることで、アプリの動作やデータを変更することが可能。<br>
これにより、致命的バグ・仕様変更に柔軟に対応することが可能。<br>


## Wiki

[Wiki](https://github.com/NNNIC/slag/wiki)  



