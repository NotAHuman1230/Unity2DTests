import websockets
import asyncio

connection = None
async def handler(websocket, path):
    while True:
        try:
            connection = websocket
            message = await websocket.recv()
            print(f"Received message: {message}")
        except: print("Connection closed")

async def main():
    async with websockets.serve(handler, "localhost", 6969):
        await asyncio.Future()  # run forever
print("hello")
asyncio.run(main())
print("hello")
while True:
    meesage = input("Enter message: ")
    connection.send(meesage)