import argparse
import subprocess


def create_parser():
    parser = argparse.ArgumentParser()
    parser.add_argument('-s', '--stop', action='store_true',
                        help='Stop containers.')

    parser.add_argument('-c', '--count', type=int, help='Number of containers to start.',
                        default='3', required=False)
    parser.add_argument('-p', '--port', type=int, help='The lowest port number for the containers.',
                        default='9000', required=False)
    return parser


if __name__ == "__main__":
    init_parser = create_parser()
    args = init_parser.parse_args()

    if args.stop:
        stdout, stderr = subprocess.Popen(["docker", "ps", "-aq", "--filter", "label=pymgmt=true"],
                                          stdout=subprocess.PIPE, stderr=subprocess.STDOUT).communicate()
        containers = stdout.decode("ascii").strip().split('\n')
        for i in containers:
            subprocess.call(["docker", "rm", "-f", i])
    else:
        for i in range(args.count):
            subprocess.call(["docker", "run", "-l", "pymgmt=true", "--name", f"wait-api_{i}",
                            "-d", "-p", f"{args.port + i}:80", "tiegris/wait-api"])
