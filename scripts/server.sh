echo ""
while true
do

	curl -sS -H "X-OS-TYPE: 2" -H "X-TOKEN: a15ff02abd1be093-1161b6fa-4e211723-b78be899-4d0a0a7b1fbe34253d43e6bd" https://app.makaiwars-sp.jp/asg/shopj/recovery | cut -d',' -f 2
	sleep 60
done
