echo ""
while true
do
	curl -sS -H "X-OS-TYPE: 2" -H "X-TOKEN: 3841c8bb34dad48c-add7fa5d-4e471597-b7c05746-230fcdce336afcaa39cc0f58" https://app.makaiwars-sp.jp/asg/shopj/recovery | cut -d',' -f 2
	sleep 60
done
