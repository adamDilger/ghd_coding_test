HOST="http://localhost:5069"

curl "$HOST/products";
echo

id=$(curl -X POST "$HOST/products" -H 'Content-Type: application/json' -d '{"Name": "test", "Price": 100, "Brand": "FUK"}' | jq -r)

echo "$id"
echo

curl "$HOST/products"
echo

curl -X PUT "$HOST/products/$id" -H 'Content-Type: application/json' -d '{"Name": "updated", "Price": 120, "Brand": "FUK"}'
echo

curl "$HOST/products";
echo