#!/bin/bash
while true; do
    stty -icanon min 0 time 100
    echo -n "是否执行迁移(yes or no)?"
    read -r Arg
    case $Arg in
    Y | y | YES | yes)
        break
        ;;
    N | n | NO | no)
        exit
        ;;
    "") #Autocontinue
        break ;;
    esac
done
while true; do
    stty -icanon min 0 time 100
    echo -n "输入迁移名称"
    read -r Arg
    case $Arg in
    "") ;;
    *)
        break
        ;;

    esac
done

echo "正在添加迁移 $Arg"
cd src || exit
dotnet ef migrations add "$Arg" --startup-project Jt808TerminalEmulator.Api --project Jt808TerminalEmulator.Data
